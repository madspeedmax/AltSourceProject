using AltSourceBank.Models;
using System.Web.Mvc;
using BankApi.Models;

namespace AltSourceBank.Controllers
{
    [Authorize]
    public class BankAccountController : Controller
    {
        private BankApi.Services.BankAccountService _bankAccountService = new BankApi.Services.BankAccountService();

        [Authorize]
        public ActionResult Accounts()
        {
            var bankAccts = _bankAccountService.GetBankAccounts(User.Identity.Name);
            return View(bankAccts);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BankAccountViewModel account)
        {
            if (ModelState.IsValid)
            {
                var newAccount = new BankAccount()
                {
                    Balance = account.Balance,
                    AccountType = account.AccountType,
                    User = User.Identity.Name
                };

                if (!_bankAccountService.CreateBankAccount(newAccount))
                {
                    ModelState.AddModelError("", "An error occured while attempting to create account");
                    return View(account);
                }
            }
            return RedirectToAction("Accounts");
        }
    }
}