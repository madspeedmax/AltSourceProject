using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankApi.Models
{
    public static class UserList
    {
        static UserList()
        {
            Users = new List<UserAccount>();
        }

        public static List<UserAccount> Users { get; set; }
    }

    public class UserAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}