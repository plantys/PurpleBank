using Microsoft.EntityFrameworkCore;
using PurpleBank.Models;
using System;

namespace PurpleBank.Data {
 public class BankingDbContext : DbContext {
  public BankingDbContext(DbContextOptions<BankingDbContext> options)
      : base(options) {
  }

  public DbSet<BankingAccountV2> Accounts { get; set; }
  public DbSet<BankingTransaction> Transactions { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
   modelBuilder.Entity<BankingAccountV2>(entity =>
   {
    entity.ToTable("BankingAccount");
    entity.HasKey(b => b.AccountId); // Define primary key for BankingAccountV2
    entity.Ignore(b => b.AdditionalProperties);
   });
      modelBuilder.Entity<BankingTransaction>(entity =>
   {
    entity.ToTable("BankingTransaction");
    entity.HasKey(b => b.TransactionId); // Define primary key for BankingTransaction
    entity.Ignore(b => b.AdditionalProperties);
   });
   modelBuilder.Entity<BankingAccountV2>().ToTable("BankingAccount");
   modelBuilder.Entity<BankingTransaction>().ToTable("BankingTransaction");
   modelBuilder.Entity<BankingAccountV2>().Ignore(b => b.AdditionalProperties);
   modelBuilder.Entity<BankingTransaction>().Ignore(b => b.AdditionalProperties);
   //Seed the database with dummy data
   modelBuilder.Entity<BankingAccountV2>().HasData(
       new BankingAccountV2 { AccountId = "1", DisplayName = "Account 1", MaskedNumber = "****1234", IsOwned = true, ProductName = "Savings Account" },
       new BankingAccountV2 { AccountId = "2", DisplayName = "Account 2", MaskedNumber = "****5678", IsOwned = true, ProductName = "Checking Account" },
       new BankingAccountV2 { AccountId = "3", DisplayName = "Account 3", MaskedNumber = "****9101", IsOwned = false, ProductName = "Checking Account" },
       new BankingAccountV2 { AccountId = "4", DisplayName = "Account 4", MaskedNumber = "****1121", IsOwned = false, ProductName = "Checking Account" },
       new BankingAccountV2 { AccountId = "5", DisplayName = "Account 5", MaskedNumber = "****3141", IsOwned = true, ProductName = "Checking Account" }
   );

   modelBuilder.Entity<BankingTransaction>().HasData(
       new BankingTransaction { TransactionId = "1", AccountId = "1", Amount = "100", Status = BankingTransactionStatus.POSTED, Description = "Deposit", ExecutionDateTime = DateTime.Now.AddDays(-5).ToString(), Type = BankingTransactionType.FEE, Reference = "REF001" },
       new BankingTransaction { TransactionId = "2", AccountId = "1", Amount = "200", Status = BankingTransactionStatus.POSTED, Description = "Withdraw", ExecutionDateTime = DateTime.Now.AddDays(-4).ToString(), Type = BankingTransactionType.INTEREST_CHARGED, Reference = "REF002" },
       new BankingTransaction { TransactionId = "3", AccountId = "2", Amount = "300", Status = BankingTransactionStatus.POSTED, Description = "Deposit", ExecutionDateTime = DateTime.Now.AddDays(-3).ToString(), Type = BankingTransactionType.PAYMENT, Reference = "REF003" },
       new BankingTransaction { TransactionId = "4", AccountId = "2", Amount = "400", Status = BankingTransactionStatus.POSTED, Description = "Withdraw", ExecutionDateTime = DateTime.Now.AddDays(-2).ToString(), Type = BankingTransactionType.TRANSFER_INCOMING, Reference = "REF004" },
       new BankingTransaction { TransactionId = "5", AccountId = "3", Amount = "500", Status = BankingTransactionStatus.POSTED, Description = "Deposit", ExecutionDateTime = DateTime.Now.AddDays(-1).ToString(), Type = BankingTransactionType.TRANSFER_OUTGOING, Reference = "REF005" }
   );
  }
 }
}
