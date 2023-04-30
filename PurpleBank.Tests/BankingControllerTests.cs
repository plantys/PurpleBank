using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PurpleBank.Controllers;
using PurpleBank.Data;
using PurpleBank.Models;
using PurpleBank;
using Xunit;

namespace PurpleBank.Tests {
 public class BankingControllerTests {
  private BankingDbContext _context;

  public BankingControllerTests() {
   _context = InMemoryDbContextFactory.GetInMemoryDbContext();
  }

  private void SeedData() {
   _context.Accounts.Add(new BankingAccountV2 { AccountId = "1", DisplayName = "Account 1", MaskedNumber = "****1234", IsOwned = true, ProductName = "Savings Account" });
   _context.Transactions.Add(new BankingTransaction { TransactionId = "1", AccountId = "1", Amount = "100", Status = BankingTransactionStatus.POSTED, Description = "Deposit", ExecutionDateTime = DateTime.Now.AddDays(-5).ToString(), Type = BankingTransactionType.FEE, Reference = "REF001" });
   _context.SaveChanges();
  }

  [Fact]
  public async Task GetAccounts_ReturnsAllAccounts() {
   // Arrange
   SeedData();
   var controller = new BankingController(_context);

   // Act
   var result = await controller.GetAccounts();

   // Assert
   var accounts = Assert.IsAssignableFrom<IEnumerable<BankingAccountV2>>(result.Value);
   Assert.Equal(1, accounts.Count());
  }

  [Fact]
  public async Task GetAccount_ReturnsSpecificAccount() {
   // Arrange
   SeedData();
   var controller = new BankingController(_context);
   string accountId = "1";

   // Act
   var result = await controller.GetAccount(accountId);

   // Assert
   var account = Assert.IsAssignableFrom<BankingAccountV2>(result.Value);
   Assert.Equal(accountId, account.AccountId);
  }
 }
}
