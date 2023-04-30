using System.Linq;
using PurpleBank.Data;
using PurpleBank.Models;


namespace PurpleBank.Tests {
 public class BankingDbContextTests {
  private BankingDbContext _context;

  public BankingDbContextTests() {
   _context = InMemoryDbContextFactory.GetInMemoryDbContext();
  }

  [Fact]
  public void AddAccount_AddsAccountToDatabase() {
   // Arrange
   var newAccount = new BankingAccountV2 { AccountId = "1", DisplayName = "Account 1", MaskedNumber = "****1234", IsOwned = true, ProductName = "Savings Account" };

   // Act
   _context.Accounts.Add(newAccount);
   _context.SaveChanges();

   // Assert
   var addedAccount = _context.Accounts.Find("1");
   Assert.NotNull(addedAccount);
   Assert.Equal(newAccount.AccountId, addedAccount.AccountId);
  }

  // Add more tests for other CRUD operations or custom logic in your BankingDbContext
 }
}
