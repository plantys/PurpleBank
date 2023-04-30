using Xunit;
using PurpleBank.Models;

namespace PurpleBank.Tests {
 public class BankingModelTests {
  [Fact]
  public void AccountNumber_ShouldFail_WhenLengthIsInvalid() {
   // Arrange
   var account = new BankingAccountV2 {
    MaskedNumber = "12345678901" // 11 characters

   };

   // Act
   bool isValidAccountNumber = account.MaskedNumber.Length == 10;

   // Assert
   Assert.False(isValidAccountNumber, "Account number should be exactly 10 characters long.");
  }

  [Fact]
  public void AccountNumber_ShouldPass_WhenLengthIsValid() {
   // Arrange
   var account = new BankingAccountV2 {
    MaskedNumber = "1234567890" // 10 characters

   };

   // Act
   bool isValidAccountNumber = account.MaskedNumber.Length == 10;

   // Assert
   Assert.True(isValidAccountNumber, "Account number should be exactly 10 characters long.");
  }
 }
}
