using Microsoft.AspNetCore.Mvc;
using PurpleBank.Data;
using PurpleBank.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurpleBank.Controllers {
 public class BankingControllerImpl : ControllerBase, IBankingController {
  private readonly BankingDbContext _context;

  public BankingControllerImpl(BankingDbContext context) {
   _context = context;
  }

  public async Task<ActionResult<IEnumerable<Account>>> GetAccounts() {
   return await _context.Accounts.ToListAsync();
  }

  public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction) {
   _context.Transactions.Add(transaction);
   await _context.SaveChangesAsync();

   return CreatedAtAction(nameof(CreateTransaction), new { id = transaction.Id }, transaction);
  }
 }
}
