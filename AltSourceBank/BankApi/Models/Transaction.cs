using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankApi.Models
{
    public enum TransactionType { Deposit, Withdrawal };

    public class Transaction
    {
        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [Range(0.00, int.MaxValue, ErrorMessage = "Amount must be 0 to 2,147,483,647")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Amount must be a positive number and no more than two decimal places")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public Guid accountId { get; set; }

        public DateTime Instant { get; set; }
    }
}