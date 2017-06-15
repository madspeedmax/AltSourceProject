using BankApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankApi.Services
{
    public class TransactionService
    {
        public bool CreateTransaction(Transaction transaction)
        {
            var account = BankAccountList.BankAccounts.FirstOrDefault(b => b.AccountId == transaction.accountId);
            if (account != null)
            {
                var newTransaction = new Transaction()
                {
                    accountId = transaction.accountId,
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    Instant = transaction.Instant
                };

                //transaction amount must be positive
                if (newTransaction.Amount < 0)
                {
                    return false;
                }

                if (newTransaction.Type == TransactionType.Deposit)
                {
                    //avoid possible overflow
                    if ((decimal.MaxValue - account.Balance) < newTransaction.Amount)
                    {
                        return false;
                    }

                    account.Balance += newTransaction.Amount;
                }
                else if (newTransaction.Type == TransactionType.Withdrawal)
                {
                    // do not allow withdrawal that would result in negative account balance
                    if (account.Balance < newTransaction.Amount)
                    {
                        return false;
                    }
                    account.Balance -= newTransaction.Amount;
                }


                account.Transactions.Add(newTransaction);

                return true;
            }

            return false;
        }

        public List<Transaction> GetTransactions(Guid accountId)
        {
            var account = BankAccountList.BankAccounts.FirstOrDefault(b => b.AccountId == accountId);
            var transactions = new List<Transaction>();

            if (account != null)
            {
                transactions = account.Transactions;
            }

            return transactions;
        }
    }
}