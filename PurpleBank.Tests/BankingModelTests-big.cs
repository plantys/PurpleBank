using Xunit;
using Microsoft.AspNetCore.Mvc;
using PurpleBank.Controllers;
using PurpleBank.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using PurpleBank;

/* test covers
1.Getting all accounts
2. Getting an account by ID
3. Getting an account by ID when the account is not found
4. Depositing a valid amount to an account
5. Depositing an invalid amount to an account
6. Withdrawing a valid amount from an account
7. Withdrawing an invalid amount from an account
8. Withdrawing an amount greater than the account balance
*/
namespace PurpleBank.Tests {
 public class BankingControllerTests {
  private readonly BankingController _controller;

  public BankingControllerTests() {
   _controller = new BankingController();
  }

  [Fact]
  public void GetAllAccounts_ReturnsAllAccounts() {
   // Act
   var result = _controller.GetAllAccounts();

   // Assert
   Assert.IsType<List<Account>>(result);
  }

  [Fact]
  public void GetAccountById_ReturnsAccountWithGivenId() {
   // Arrange
   int accountId = 1;

   // Act
   var result = _controller.GetAccountById(accountId);

   // Assert
   var okResult = Assert.IsType<OkObjectResult>(result.Result);
   var account = Assert.IsType<Account>(okResult.Value);
   Assert.Equal(accountId, account.Id);
  }

  [Fact]
  public void GetAccountById_AccountNotFound_ReturnsNotFound() {
   // Arrange
   int accountId = -1;

   // Act
   var result = _controller.GetAccountById(accountId);

   // Assert
   Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void Deposit_ValidAmount_AccountUpdated() {
   // Arrange
   int accountId = 1;
   decimal depositAmount = 100m;
   var initialAccount = _controller.GetAccountById(accountId).Result as OkObjectResult;

   // Act
   var result = _controller.Deposit(accountId, depositAmount);

   // Assert
   var okResult = Assert.IsType<OkObjectResult>(result.Result);
   var updatedAccount = Assert.IsType<Account>(okResult.Value);
   Assert.Equal(((Account)initialAccount.Value).Balance + depositAmount, updatedAccount.Balance);
  }

  [Fact]
  public void Deposit_InvalidAmount_ReturnsBadRequest() {
   // Arrange
   int accountId = 1;
   decimal depositAmount = -100m;

   // Act
   var result = _controller.Deposit(accountId, depositAmount);

   // Assert
   Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public void Withdraw_ValidAmount_AccountUpdated() {
   // Arrange
   int accountId = 1;
   decimal withdrawalAmount = 100m;
   var initialAccount = _controller.GetAccountById(accountId).Result as OkObjectResult;

   // Act
   var result = _controller.Withdraw(accountId, withdrawalAmount);

   // Assert
   var okResult = Assert.IsType<OkObjectResult>(result.Result);
   var updatedAccount = Assert.IsType<Account>(okResult.Value);
   Assert.Equal(((Account)initialAccount.Value).Balance - withdrawalAmount, updatedAccount.Balance);
  }

  [Fact]
  public void Withdraw_InvalidAmount_ReturnsBadRequest() {
   // Arrange
   int accountId = 1;
   decimal withdrawalAmount = -100m;

   // Act
   var result = _controller.Withdraw(accountId, withdrawalAmount);

   // Assert
   Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public void Withdraw_InsufficientFunds_ReturnsBadRequest() {
   // Arrange
   int accountId = 1;
   decimal withdrawalAmount = 100000m;

   // Act
   var result = _controller.Withdraw(accountId, withdrawalAmount);

   // Assert
   Assert.IsType < BadRequestObjectResult > (result.Result);
  }
 }
}