using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankApi.Models
{
    public static class BankAccountList
    {
        static BankAccountList()
        {
            BankAccounts = new List<BankAccount>();
        }

        public static List<BankAccount> BankAccounts { get; set; }
    }

    public enum BankAccountType { Checking, Savings };

    public class BankAccount
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        public BankAccountType AccountType;

        [DataType(DataType.Currency)]
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