using AltSourceBank.Models;
using System.Web.Mvc;
using System.Web.Security;
using BankApi.Services;

namespace AltSourceBank.Controllers
{
    public class UserAccountController : Controller
    {
        private UserAccountService _UserService = new BankApi.Services.UserAccountService();

        // GET: UserAccount
        public ActionResult Index()
        {
            return RedirectToAction("Login", "UserAccount");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Accounts", "BankAccount");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_UserService.ValidateLogin(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Accounts", "BankAccount");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //make sure email/username is unique
                if (!_UserService.IsUsernameUnique(model.Email))
                {
                    ModelState.AddModelError("", "There is already an account with that email.");
                    return View(model);
                }

                var user = new BankApi.Models.UserAccount() { Email = model.Email, Password = model.Password };

                if (_UserService.Create(user))
                {
                    return RedirectToAction("Index", "Home");
                } 
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}