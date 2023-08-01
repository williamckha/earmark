using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Earmark.WebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("")]
        public IEnumerable<AccountDTO> GetAccounts()
        {
            return _accountService.GetAccounts().Select(x => AccountToDTO(x));
        }

        [HttpGet("{id:int}")]
        public ActionResult<AccountDTO> GetAccount(int id)
        {
            var account = _accountService.GetAccounts().FirstOrDefault(x => x.Id == id);

            if (account is null)
            {
                return NotFound();
            }

            return AccountToDTO(account);
        }

        [HttpGet("~/api/transactions")]
        public ActionResult<IEnumerable<TransactionDTO>> GetTransactions()
        {
            return Ok(_accountService.GetTransactions().Select(x => TransactionToDTO(x)));
        }
        
        [HttpGet("{accountId:int}/transactions")]
        public ActionResult<IEnumerable<TransactionDTO>> GetTransactions(int accountId)
        {
            return Ok(_accountService.GetTransactions(new[] { accountId }).Select(x => TransactionToDTO(x)));
        }

        [HttpGet("~/api/transactions/{id:int}")]
        public ActionResult<TransactionDTO> GetTransaction(int id)
        {
            var transaction = _accountService.GetTransactions().FirstOrDefault(x => x.Id == id);

            if (transaction is null)
            {
                return NotFound();
            }

            return TransactionToDTO(transaction);
        }

        [HttpPost("{accountId:int}/transactions")]
        public ActionResult<TransactionDTO> AddTransaction(int accountId, TransactionDTO transaction)
        {
            try
            {
                var createdTransaction = _accountService.AddTransaction(accountId, transaction.Date, transaction.Amount);
                var transactionDTO = TransactionToDTO(createdTransaction);
                return CreatedAtAction(nameof(GetTransaction), new { id = transactionDTO.Id }, transactionDTO);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpDelete("~/api/transactions/{id:int}")]
        public ActionResult DeleteTransaction(int id)
        {
            try
            {
                _accountService.RemoveTransaction(id);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        private AccountDTO AccountToDTO(Account account)
        {
            return new AccountDTO()
            {
                Id = account.Id,
                Name = account.Name,
                TotalBalance = account.TotalBalance
            };
        }

        private TransactionDTO TransactionToDTO(Transaction transaction)
        {
            return new TransactionDTO()
            {
                Id = transaction.Id,
                Date = transaction.Date,
                Memo = transaction.Memo,
                Amount = transaction.Amount,
                AccountName = transaction.Account.Name,
                PayeeName = transaction.Payee?.Name,
                CategoryName = transaction.Category?.Name,
            };
        }
    }
}
