using BankApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankApi.Services
{
    public class BankAccountService
    {
        public List<BankAccount> GetBankAccounts(string username)
        {
            return BankAccountList.BankAccounts.Where(b => b.User == username).ToList();
        }

        public BankAccount GetAccount(Guid accountId)
        {
            var account = BankAccountList.BankAccounts.FirstOrDefault(b => b.AccountId == accountId);
            return account;
        }

        public bool CreateBankAccount(BankAccount bankAccount)
        {
            if (bankAccount.User != null && UserList.Users.Any(u => u.Email == bankAccount.User))
            {
                var newAccount = new BankAccount()
                {
                    User = bankAccount.User,
                    AccountType = bankAccount.AccountType,
                    Balance = bankAccount.Balance
                };
                BankAccountList.BankAccounts.Add(newAccount);
                return true;
            }

            return false;
        }
    }
}