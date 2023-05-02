using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBank.Data;
using PurpleBank.Models;
//just checking latest commit
namespace PurpleBank.Controllers {
 [ApiController]
 [Route("[controller]")]
 public class BankingController : ControllerBase {
  private readonly BankingDbContext _context;

  public BankingController(BankingDbContext context) {
   _context = context;
  }

  // GET: Banking
  [HttpGet]
  public async Task<ActionResult<IEnumerable<BankingAccountV2>>> GetAccounts() {
   return await _context.Accounts.ToListAsync();
  }

  // GET: Banking/5
  [HttpGet("{id}")]
  public async Task<ActionResult<BankingAccountV2>> GetAccount(string id) {
   var account = await _context.Accounts.FindAsync(id);

   if (account == null) {
    return NotFound();
   }

   return account;
  }

  // PUT: Banking/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateAccount(string id, BankingAccountV2 account) {
   if (id != account.AccountId) {
    return BadRequest();
   }

   _context.Entry(account).State = EntityState.Modified;

   try {
    await _context.SaveChangesAsync();
   } catch (DbUpdateConcurrencyException) {
    if (!AccountExists(id)) {
     return NotFound();
    } else {
     throw;
    }
   }

   return NoContent();
  }

  // POST: Banking
  [HttpPost]
  public async Task<ActionResult<BankingAccountV2>> CreateAccount(BankingAccountV2 account) {
   _context.Accounts.Add(account);
   await _context.SaveChangesAsync();

   return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
  }

  // DELETE: Banking/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteAccount(string id) {
   var account = await _context.Accounts.FindAsync(id);
   if (account == null) {
    return NotFound();
   }

   _context.Accounts.Remove(account);
   await _context.SaveChangesAsync();

   return NoContent();
  }

  private bool AccountExists(string id) {
   return _context.Accounts.Any(e => e.AccountId == id);
  }
 }
}
