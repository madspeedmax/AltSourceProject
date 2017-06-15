using BankApi.Aspects;
using BankApi.Models;
using BankApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace BankApi.Controllers
{
    public class BankAccountController : ApiController
    {
        BankAccountService _bankAccountService = new BankAccountService();
        TransactionService _transactionService = new TransactionService();

        [HttpGet]
        [BankApiAspect]
        public List<BankAccount> GetAccounts()
        {
            var accounts = new List<BankAccount>();
            var headers = HttpContext.Current.Request.Headers;
            var username = GetUsernameFromAuthHeader(headers["Authorization"]);

            if (!string.IsNullOrWhiteSpace(username))
            {
                accounts = _bankAccountService.GetBankAccounts(username);
            }
            
            return accounts;
        }

        [HttpPost]
        [BankApiAspect]
        [Route("api/BankAccount/CreateAccount")]
        public bool CreateAccount(BankAccount account)
        {
            if (account == null)
            {
                return false;
            }

            var headers = HttpContext.Current.Request.Headers;
            var username = GetUsernameFromAuthHeader(headers["Authorization"]);

            if (!string.IsNullOrWhiteSpace(username))
            {
                var newAccount = new BankAccount()
                {
                    AccountId = Guid.NewGuid(),
                    User = username,
                    Balance = account.Balance,
                    AccountType = account.AccountType
                };

                return _bankAccountService.CreateBankAccount(newAccount);
            }

            return false;
        }

        [HttpGet]
        [BankApiAspect]
        public List<Transaction> GetTransactions(Guid accountId)
        {
            var headers = HttpContext.Current.Request.Headers;
            var username = GetUsernameFromAuthHeader(headers["Authorization"]);
            var account = new BankAccount();
            var transactions = new List<Transaction>();

            if (accountId != null)
            {
                account = _bankAccountService.GetAccount(accountId);
            }

            if (account != null && account.User == username)
            {
                transactions = account.Transactions;
            }

            return transactions;
        }

        [HttpPost]
        [BankApiAspect]
        [Route("api/BankAccount/CreateTransaction")]
        public bool CreateTransaction(Transaction transaction)
        {

            if (transaction == null)
            {
                return false;
            }

            var headers = HttpContext.Current.Request.Headers;
            var username = GetUsernameFromAuthHeader(headers["Authorization"]);

            if (!string.IsNullOrWhiteSpace(username) && transaction.accountId != null)
            {
                var account = _bankAccountService.GetAccount(transaction.accountId);

                if (account != null && account.User == username)
                {
                    var newTransaction = new Transaction()
                    {
                        accountId = transaction.accountId,
                        Type = transaction.Type,
                        Amount = transaction.Amount,
                        Instant = transaction.Instant
                    };

                    return _transactionService.CreateTransaction(newTransaction);
                }
            }

            return false;
        }

        private string GetUsernameFromAuthHeader(string authHeader)
        {
            var username = "";
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
            {
                authHeader = authHeader.Substring("Basic ".Length);
            }

            if (!string.IsNullOrEmpty(authHeader))
            {

                authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

                var tokens = authHeader.Split(':');
                if (tokens.Length >= 2)
                {
                    username = tokens[0];
                }
            }

            return username;
        }
    }
}
