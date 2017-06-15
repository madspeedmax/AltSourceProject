using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankConsole.Models
{
    public enum TransactionType { Deposit, Withdrawal };

    public class Transaction
    {
        public TransactionType Type { get; set; }

        public decimal Amount { get; set; }

        public Guid accountId { get; set; }

        public DateTime Instant { get; set; }
    }
}
