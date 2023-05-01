#!/bin/bash

# Variables
resourceGroupName="rg-purplebank"
location="australiaeast"
vnetName="vnet-purplebank"
gatewaySubnet="subnet-gateway"
appgwSubnet="subnet-appgw"
apimSubnet="subnet-apim"
cosmosdbSubnet="subnet-cosmosdb"
storageSubnet="subnet-storage"
appsSubnet="subnet-apps"
pipName="pip-purplebank-gateway"
appgwName="appgw-purplebank"
cosmosdbName="cosmosdb-purplebank"
storageAccountName="storagepurplebank"
sqlServerName="sqlserver-purplebank"
sqlDatabaseName="db-purplebank"
aksName="aks-purplebank"
systemNodePoolName="nodepool-system"
userNodePoolName="nodepool-user"
apimName="apim-purplebank"
aadName="aad-purplebank"
frontDoorName="frontdoor-purplebank"
frontDoorBackendPoolName="backend-pool-purplebank"
frontDoorRoutingRuleName="routing-rule-purplebank"
frontDoorHealthProbeName="health-probe-purplebank"
frontDoorEndpointName="frontend-endpoint-purplebank"
frontDoorCustomDomainName="custom-domain-purplebank"
customDomainHostName="purplebank.com"
aadAdminGroupDisplayName="AKS Administrators"
aadContributorRoleId=$(az ad sp show --id 00000002-0000-0000-c000-000000000000 --query objectId --output tsv)
kvName="kv-purplebank"
lawName="law-purplebank"

# Create resource group
az group create --name $resourceGroupName --location $location

# Create virtual network
az network vnet create --name $vnetName --resource-group $resourceGroupName --address-prefixes 10.0.0.0/16 --subnet-name $gatewaySubnet --subnet-prefixes 10.0.0.0/24
az network vnet subnet create --name $appgwSubnet --vnet-name $vnetName --resource-group $resourceGroupName --address-prefixes 10.0.1.0/24
az network vnet subnet create --name $apimSubnet --vnet-name $vnetName --resource-group $resourceGroupName --address-prefixes 10.0.2.0/24
az network vnet subnet create --name $cosmosdbSubnet --vnet-name $vnetName --resource-group $resourceGroupName --address-prefixes 10.0.3.0/24
az network vnet subnet create --name $storageSubnet --vnet-name $vnetName --resource-group $resourceGroupName --address-prefixes 10.0.4.0/24
az network vnet subnet create --name $appsSubnet --vnet-name $vnetName --resource-group $resourceGroupName --address-prefixes 10.0.5.0/24

# Create public IP address
az network public-ip create --name $pipName --resource-group $resourceGroupName --sku Standard --allocation-method Static

# Create application gateway
az network application-gateway create --name $appgwName --resource-group $resourceGroupName --location $location --sku Standard_v2 --public-ip-address $pipName --vnet-name $vnetName --subnet $appgwSubnet --frontend-port 80 --http-settings-cookie-based-affinity Disabled --http-settings-port 80 --http-settings-protocol Http --routing-rule-type Basic --enable-waf true

# Configure application gateway to only communicate with API Management
az network application-gateway http-settings update --gateway-name $appgwName --name appGateway

# Create Cosmos DB account
az cosmosdb create --name $cosmosdbName --resource-group $resourceGroupName --locations "australiaeast=0" --kind GlobalDocumentDB --default-consistency-level "Session" --enable-multiple-write-locations true --ip-range-filter "" --enable-public-network false --locations-write "australiaeast" --locations-read "australiaeast"

# Create storage account
az storage account create --name $storageAccountName --resource-group $resourceGroupName --location $location --sku Standard_LRS --kind StorageV2 --access-tier Hot --default-action Allow

# Create Azure SQL Server
sqlPassword=$(az ad sp create-for-rbac --name $aadName --role contributor --query password --output tsv)
az sql server create --name $sqlServerName --resource-group $resourceGroupName --location $location --admin-user sqladmin --admin-password $sqlPassword
az sql server firewall-rule create --name "AllowAllWindowsAzureIps" --resource-group $resourceGroupName --server $sqlServerName --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
az sql db create --name $sqlDatabaseName --resource-group $resourceGroupName --server $sqlServerName --service-objective S0

# Create AKS cluster
az aks create --name $aksName --resource-group $resourceGroupName --location $location --generate-ssh-keys --network-plugin azure --vnet-subnet-id "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/virtualNetworks/{vnetName}/subnets/{systemNodePoolName}" --service-cidr 10.0.0.0/16 --dns-service-ip 10.0.0.10 --docker-bridge-address 172.17.0.1/16 --nodepool-name $systemNodePoolName --node-count 1 --node-vm-size Standard_DS2_v2 --node-osdisk-size 30 --node-taints CriticalAddonsOnly=true:NoSchedule --enable-managed-identity
az aks nodepool add --name $userNodePoolName --resource-group $resourceGroupName --cluster-name $aksName --node-count 1 --node-vm-size Standard_DS2_v2 --os-disk-size 30 --node-taints workload=user:NoSchedule --vnet-subnet-id "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/virtualNetworks/{vnetName}/subnets/{userNodePoolName}" --tags environment=dev --mode User --enable-auto-scaling --min-count 1 --max-count 3

# Create API Management service
az apim create --name $apimName --resource-group $resourceGroupName --location $location --sku-name Consumption --publisher-email admin@$customDomainHostName --publisher-name "PurpleBank" --virtual-network $vnetName --subnet $apimSubnet

# Create Azure AD tenant
az ad tenant create --display-name $aadName --is-federation false --initial-domain-name $customDomainHostName

# Create Azure AD admin group
aadAdminGroupId=$(az ad group create --display-name "$aadAdminGroupDisplayName" --mail-nickname "aks-admins" --query objectId --output tsv)
az ad group member add --group $aadAdminGroupId --member-id $aadContributorRoleId

# Create Azure Front Door
az network front-door create --name $frontDoorName --resource-group $resourceGroupName --backend-pool $frontDoorBackendPoolName --routing-rule $frontDoorRoutingRuleName --health-probe $frontDoorHealthProbeName --frontend-endpoint $frontDoorEndpoint

# Create Azure Front Door backend pool
az network front-door backend-pool create --name $frontDoorBackendPoolName --resource-group $resourceGroupName --front-door-name $frontDoorName --backend-host-header $cosmosdbName.documents.azure.com --address $cosmosdbName.documents.azure.com --session-affinity-enabled true --load-balancing disabled

# Create Azure Front Door routing rule
az network front-door routing-rule create --name $frontDoorRoutingRuleName --resource-group $resourceGroupName --front-door-name $frontDoorName --frontend-endpoints $frontDoorEndpointName --accepted-protocols Https --patterns / --backend-pool $frontDoorBackendPoolName --route-type Forward --custom-forwarding-path ""

# Create Azure Front Door health probe
az network front-door probe create --name $frontDoorHealthProbeName --resource-group $resourceGroupName --front-door-name $frontDoorName --interval 30 --path / --protocol Https --threshold 3 --timeout 5

# Create Azure Front Door frontend endpoint
az network front-door frontend-endpoint create --name $frontDoorEndpointName --resource-group $resourceGroupName --front-door-name $frontDoorName --host-name $customDomainHostName --session-affinity-enabled true --session-affinity-duration 7200 --web-application-firewall-policy "" --enabled-state Enabled --priority 1 --session-affinity-cookie-name ""

# Create custom domain for Azure Front Door
az network front-door custom-domain create --name $frontDoorCustomDomainName --resource-group $resourceGroupName --front-door-name $frontDoorName --host-name $customDomainHostName

# Get Azure Key Vault ID
kvId=$(az keyvault show --name $kvName --resource-group $resourceGroupName --query id --output tsv)

# Get Log Analytics Workspace ID
lawId=$(az monitor log-analytics workspace show --workspace-name $lawName --resource-group $resourceGroupName --query id --output tsv)

# Grant permissions to Azure Key Vault for AKS managed identity
az keyvault set-policy --name $kvName --resource-group $resourceGroupName --secret-permissions get --object-id $(az aks show --name $aksName --resource-group $resourceGroupName --query identityProfile.kubeletidentity.objectId -o tsv)

# Grant permissions to Log Analytics Workspace for AKS managed identity
az monitor log-analytics workspace update --ids $lawId --update-storage-account-associations "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}"=logs --update-storage-account-associations-status Enabled
az role assignment create --assignee $(az aks show --name $aksName --resource-group $resourceGroupName --query identityProfile.kubeletidentity.objectId -o tsv) --role "Log Analytics Contributor" --scope $lawId

# Output variables
echo "export RG_NAME=$resourceGroupName" >> variables.env
echo "export LOCATION=$location" >> variables.env
echo "export VNET_NAME=$vnetName" >> variables.env
echo "export APPGW_NAME=$appgwName" >> variables.env
echo "export APIM_NAME=$apimName" >> variables.env
echo "export COSMOSDB_NAME=$cosmosdbName" >> variables.env
echo "export STORAGE_ACCOUNT_NAME=$storageAccountName" >> variables.env
echo "export SQL_SERVER_NAME=$sqlServerName" >> variables.env
echo "export SQL_DATABASE_NAME=$sqlDatabaseName" >> variables.env
echo "export AKS_NAME=$aksName" >> variables.env
echo "export KV_NAME=$kvName" >> variables.env
echo "export LAW_NAME=$lawName" >> variables.env
echo "export AAD_ADMIN_GROUP_ID=$aadAdminGroupId" >> variables.env
echo "export AAD_TENANT_ID=$aadTenantId" >> variables.env
echo "export AAD_CLIENT_ID=$aadClientId" >> variables.env
echo "export AAD_CLIENT_SECRET=$aadClientSecret" >> variables.env
echo "export FRONT_DOOR_NAME=$frontDoorName" >> variables.env
echo "export FRONT_DOOR_ENDPOINT_NAME=$frontDoorEndpointName" >> variables.env
echo "export FRONT_DOOR_CUSTOM_DOMAIN_NAME=$frontDoorCustomDomainName" >> variables.env
echo "export COSMOSDB_CONN_STRING=$(az cosmosdb keys list --name $cosmosdbName --resource-group $resourceGroupName --type connection-strings --query connectionStrings[0].connectionString -o tsv)" >> variables.env
echo "export STORAGE_CONN_STRING=$(az storage account show-connection-string --name $storageAccountName --resource-group $resourceGroupName --query connectionString -o tsv)" >> variables.env
echo "export SQL_SERVER_ADMIN_LOGIN=$(az sql server show --name $sqlServerName --resource-group $resourceGroupName --query administratorLogin -o tsv)" >> variables.env
echo "export SQL_SERVER_ADMIN_PASSWORD=$sqlPassword" >> variables.env
echo "export SQL_SERVER_FQDN=$sqlServerName.database.windows.net" >> variables.env
echo "export KEYVAULT_ID=$kvId" >> variables.env
echo "export LAW_ID=$lawId" >> variables.env

#Save variables to key vault
az keyvault secret set --name PurpleBank-variables --value "$(cat variables.env)" --vault-name $kvName

# Clean up
rm -f variables.env

echo "Resource deployment completed successfully."
