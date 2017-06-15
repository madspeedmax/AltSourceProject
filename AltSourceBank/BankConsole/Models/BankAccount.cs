using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankConsole.Models
{
    public enum BankAccountType { Checking, Savings };

    public class BankAccount
    {
        public Guid AccountId { get; set; }

        public string User { get; set; }

        public BankAccountType AccountType;

        public decimal Balance { get; set; }

        public List<Transaction> Transactions { get; set; }

        public BankAccount()
        {
            this.AccountId = Guid.NewGuid();
            this.Balance = 0.00m;
            this.Transactions = new List<Transaction>();
        }
    }
}
