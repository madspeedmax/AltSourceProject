using BankApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BankApi.Services
{
    public class UserAccountService
    {
        public UserAccountService()
        {

        }

        public bool Create(UserAccount user)
        {
            if (!string.IsNullOrWhiteSpace(user.Email) && !string.IsNullOrWhiteSpace(user.Password) && !UserList.Users.Any(u => u.Email == user.Email))
            {
                UserList.Users.Add(user);
                return true;
            }

            return false;
        }

        public bool IsUsernameUnique(string username)
        {
            return !UserList.Users.Any(u => u.Email == username);
        }

        public bool ValidateLogin()
        {
            var headers = HttpContext.Current.Request.Headers;

            if (headers == null)
            {
                return false;
            }

            var authHeader = headers["Authorization"];

            string username = null;
            string password = null;
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
                    password = tokens[1];

                    if (UserList.Users.Any(u => u.Email == username && u.Password == password))
                    {
                        return true;
                    }
                }
            }

            //if we get here, something wasn't correct with credentials
            return false;
        }

        public bool ValidateLogin(string username, string password)
        {
            if (UserList.Users.Any(u => u.Email == username && u.Password == password))
            {
                return true;
            }
            return false;
        }
    }
}