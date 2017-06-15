using BankApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AltSourceBank.Models
{
    public class BankAccountViewModel
    {
        [Required]
        [Display(Name = "Account Type")]
        public BankAccountType AccountType { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [Range(0.00, 999999999, ErrorMessage = "Initial Balance must be 0 to 999999999.00")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Balance must be a positive number and no more than two decimal places")]
        [Display(Name = "Initial Balance")]
        public decimal Balance { get; set; }

        public BankAccountViewModel()
        {
            this.AccountType = BankApi.Models.BankAccountType.Checking;
            this.Balance = 0.00m;
        }
    }
}