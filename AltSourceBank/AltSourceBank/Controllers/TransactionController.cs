using BankApi.Services;
using System;
using System.Web.Mvc;
using System.Web.Security;
using BankApi.Models;

namespace AltSourceBank.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private TransactionService _transactionService = new TransactionService();
        private BankAccountService _bankAccountService = new BankAccountService();

        public ActionResult Index()
        {
            return RedirectToAction("Accounts", "BankAccount");
        }

        public ActionResult Deposit(Guid accountId)
        {
            var account = _bankAccountService.GetAccount(accountId);

            if (account != null && account.User == User.Identity.Name)
            {
                var transaction = new Transaction() { accountId = accountId };
                return View();
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit(Transaction transaction)
        {
            var account = _bankAccountService.GetAccount(transaction.accountId);
            if (account != null && account.User == User.Identity.Name)
            {
                transaction.Type = TransactionType.Deposit;
                transaction.Instant = DateTime.Now;

                if (_transactionService.CreateTransaction(transaction))
                {
                    return RedirectToAction("Accounts", "BankAccount");
                }
                else
                {
                    ModelState.AddModelError("", "An error occured while attempting to create deposit");
                    return View(transaction);
                }
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserAccount");
        }

        public ActionResult Withdraw(Guid accountId)
        {
            var account = _bankAccountService.GetAccount(accountId);

            if (account != null && account.User == User.Identity.Name)
            {
                var transaction = new Transaction() { accountId = accountId };
                return View();
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdraw(Transaction transaction)
        {
            var account = _bankAccountService.GetAccount(transaction.accountId);
            if (account != null && account.User == User.Identity.Name)
            {
                if (transaction.Amount > account.Balance)
                {
                    ModelState.AddModelError("", "Withdraw amount cannot exceed account balance");
                    return View(transaction);
                }
                else
                {
                    transaction.Type = TransactionType.Withdrawal;
                    transaction.Instant = DateTime.Now;

                    if (_transactionService.CreateTransaction(transaction))
                    {
                        return RedirectToAction("Accounts", "BankAccount");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occured while attempting to create withdrawal");
                        return View(transaction);
                    }
                }
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserAccount");
        }

        public ActionResult Transactions(Guid accountId)
        {
            var account = _bankAccountService.GetAccount(accountId);
            if (account != null && account.User == User.Identity.Name)
            {
                var transactions = account.Transactions;
                return View(transactions);
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserAccount");
        }
    }
}