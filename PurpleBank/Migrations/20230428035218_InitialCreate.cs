using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PurpleBank.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankingAccount",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreationDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenStatus = table.Column<int>(type: "int", nullable: false),
                    IsOwned = table.Column<bool>(type: "bit", nullable: false),
                    AccountOwnership = table.Column<int>(type: "int", nullable: false),
                    MaskedNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategory = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingAccount", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "BankingTransaction",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDetailAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostingDateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueDateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionDateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Crn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApcaNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingTransaction", x => x.TransactionId);
                });

            migrationBuilder.InsertData(
                table: "BankingAccount",
                columns: new[] { "AccountId", "AccountOwnership", "CreationDate", "DisplayName", "IsOwned", "MaskedNumber", "Nickname", "OpenStatus", "ProductCategory", "ProductName" },
                values: new object[,]
                {
                    { "1", 0, null, "Account 1", true, "****1234", null, 1, 0, "Savings Account" },
                    { "2", 0, null, "Account 2", true, "****5678", null, 1, 0, "Checking Account" },
                    { "3", 0, null, "Account 3", false, "****9101", null, 1, 0, "Checking Account" },
                    { "4", 0, null, "Account 4", false, "****1121", null, 1, 0, "Checking Account" },
                    { "5", 0, null, "Account 5", true, "****3141", null, 1, 0, "Checking Account" }
                });

            migrationBuilder.InsertData(
                table: "BankingTransaction",
                columns: new[] { "TransactionId", "AccountId", "Amount", "ApcaNumber", "BillerCode", "BillerName", "Crn", "Currency", "Description", "ExecutionDateTime", "IsDetailAvailable", "MerchantCategoryCode", "MerchantName", "PostingDateTime", "Reference", "Status", "Type", "ValueDateTime" },
                values: new object[,]
                {
                    { "1", "1", "100", null, null, null, null, null, "Deposit", "23/04/2023 1:52:18 PM", false, null, null, null, "REF001", 1, 1, null },
                    { "2", "1", "200", null, null, null, null, null, "Withdraw", "24/04/2023 1:52:18 PM", false, null, null, null, "REF002", 1, 2, null },
                    { "3", "2", "300", null, null, null, null, null, "Deposit", "25/04/2023 1:52:18 PM", false, null, null, null, "REF003", 1, 5, null },
                    { "4", "2", "400", null, null, null, null, null, "Withdraw", "26/04/2023 1:52:18 PM", false, null, null, null, "REF004", 1, 6, null },
                    { "5", "3", "500", null, null, null, null, null, "Deposit", "27/04/2023 1:52:18 PM", false, null, null, null, "REF005", 1, 7, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankingAccount");

            migrationBuilder.DropTable(
                name: "BankingTransaction");
        }
    }
}
