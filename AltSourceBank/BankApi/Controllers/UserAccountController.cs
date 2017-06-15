using BankApi.Aspects;
using BankApi.Models;
using BankApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BankApi.Controllers
{
    public class UserAccountController : ApiController
    {
        public UserAccountService UserAccountService = new UserAccountService();

        [HttpPost]
        public bool Create(UserAccount user)
        {
            return UserAccountService.Create(user);
        }

        [HttpGet]
        public bool ValidateLogin()
        {
            return UserAccountService.ValidateLogin();
        }
    }
}
