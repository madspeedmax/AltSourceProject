using AltSourceBank.Models;
using BankConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BankConsole
{
    class Program
    {
        public const string localApiDomain = "http://localhost:51540/api/";
        static string username = "";
        static string password = "";
        static bool isLoggedIn = false;

        static void Main(string[] args)
        {
            string line = "";
       
            bool exit = false;
            while (!exit)
            {
                initialMenu();
                line = Console.ReadLine();
                switch(line)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        ViewAccounts();
                        break;
                    case "4":
                        CreateAccount();
                        break;
                    case "5":
                        CreateTransaction(TransactionType.Deposit);
                        break;
                    case "6":
                        CreateTransaction(TransactionType.Withdrawal);
                        break;
                    case "7":
                        ViewTransactions();
                        break;
                    case "8":
                        Logout();
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        
        public static void initialMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. View Bank Accounts");
            Console.WriteLine("4. Create new Bank Account");
            Console.WriteLine("5. Deposit");
            Console.WriteLine("6. Withdraw");
            Console.WriteLine("7. View Transactions");
            Console.WriteLine("8. Logout");
            Console.WriteLine("Type \"exit\" to leave application");
            Console.WriteLine();
        }

        public static void Login()
        {
            Console.WriteLine();
            Console.WriteLine("LOGIN");
            Console.WriteLine();
            Console.WriteLine("Enter Email:");
            username = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            password = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                string url = localApiDomain + "useraccount/validateLogin";
                var serializer = new JavaScriptSerializer();

                bool result = false;

                try
                {
                    result = serializer.Deserialize<bool>(GetApiCall(url, username, password));
                }
                catch
                {
                    Console.WriteLine("An error occured while attempting to log in");
                    return;
                }
                
                if (result)
                {
                    isLoggedIn = true;
                    Console.WriteLine("Successfully logged in");
                    return;
                }
            }

            Console.WriteLine("Login failed");
        }

        public static void Logout()
        {
            isLoggedIn = false;
            username = "";
            password = "";
        }

        public static void Register()
        {
            string regUsername = "";
            string regPassword = "";
            string regConfirmPassword = "";
            Console.WriteLine();
            Console.WriteLine("REGISTER");
            Console.WriteLine();
            Console.WriteLine("Enter Email:");
            regUsername = Console.ReadLine();

            if (!IsValidEmail(regUsername))
            {
                Console.WriteLine("Please use a valid email address");
                return;
            }

            Console.WriteLine("Enter Password:");
            regPassword = Console.ReadLine();
            Console.WriteLine("Confirm Password:");
            regConfirmPassword = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(regUsername) && !string.IsNullOrWhiteSpace(regPassword) && regPassword == regConfirmPassword)
            {
                string url = localApiDomain + "useraccount/create";
                var serializer = new JavaScriptSerializer();
                var userAccount = new UserAccount()
                {
                    Email = regUsername,
                    Password = regPassword
                };
                var data = serializer.Serialize(userAccount);

                bool result = false;

                try
                {
                    result = serializer.Deserialize<bool>(PostApiCall(url, username, password, data));
                }
                catch
                {
                    Console.WriteLine("An error occured while creating new user account");
                    return;
                }
                
                if (result)
                {
                    Console.WriteLine("Successfully registered");
                    return;
                }
            }

            Console.WriteLine("Register failed");
        }

        public static void ViewAccounts()
        {
            Console.WriteLine();

            if (!isLoggedIn)
            {
                Console.WriteLine("Please log in\n");
                return;
            }

            string url = localApiDomain + "bankaccount/GetAccounts";
            var serializer = new JavaScriptSerializer();

            var result = new List<BankAccount>();

            try
            {
                result = serializer.Deserialize<List<BankAccount>>(GetApiCall(url, username, password));
            }
            catch
            {
                Console.WriteLine("An error occured while fetching accounts");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("ACCOUNTS");
            string resultFormat = "|{0,-40}|{1,-10}|{2,-20}|";
            Console.WriteLine(string.Format(resultFormat, "Account Id", "Type", "Balance"));
            foreach (var account in result)
            {
                Console.WriteLine(string.Format(resultFormat, account.AccountId.ToString(), account.AccountType.ToString(), account.Balance.ToString()));
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        public static void CreateAccount()
        {
            Console.WriteLine();

            if (!isLoggedIn)
            {
                Console.WriteLine("Please log in\n");
                return;
            }

            string balance;
            decimal decimalBalance;
            string accountType;
            int intAccountType;
            Console.WriteLine("Enter account type (\"0\" for checking or \"1\" for savings");
            accountType = Console.ReadLine();
            Console.WriteLine("Enter initial balance:");
            balance = Console.ReadLine();

            if (accountType != "0" && accountType != "1")
            {
                Console.WriteLine("Invalid account type");
                return;
            }

            int.TryParse(accountType, out intAccountType);

            if (decimal.TryParse(balance, out decimalBalance))
            {
                if (decimalBalance > decimal.MaxValue || (decimalBalance % .01m != 0) || decimalBalance < 0)
                {
                    Console.WriteLine(string.Format("Initial balance must be positive, smaller than {0} and divisble by 0.01", decimal.MaxValue.ToString()));
                    return;
                }
            }
            else
            {
                Console.WriteLine("Initial balance is not valid");
                return;
            }

            var newAccount = new BankAccount()
            {
                AccountType = (BankAccountType) intAccountType,
                Balance = decimalBalance,
                User = username
            };

            string url = localApiDomain + "bankaccount/createaccount";
            var serializer = new JavaScriptSerializer();
            var data = serializer.Serialize(newAccount);

            bool result = false;

            try
            {
                result = serializer.Deserialize<bool>(PostApiCall(url, username, password, data));
            }
            catch
            {
                Console.WriteLine("An error occured while attempting to create account");
                return;
            }


            if (result)
            {
                Console.WriteLine("Successfully created account");
                return;
            }

            Console.WriteLine("Account creation failed");
        }

        public static void CreateTransaction(TransactionType type)
        {
            Console.WriteLine();

            if (!isLoggedIn)
            {
                Console.WriteLine("Please log in\n");
                return;
            }

            Console.WriteLine("Enter account id");
            string accountId = Console.ReadLine();
            Guid guidAccountId;

            if (string.IsNullOrWhiteSpace(accountId))
            {
                Console.WriteLine("Account Id is required");
                return;
            }

            if (!Guid.TryParse(accountId, out guidAccountId))
            {
                Console.WriteLine("Invalid account Id");
                return;
            }

            string amount;
            decimal decimalAmount;

            Console.WriteLine("Enter transaction amount");
            amount = Console.ReadLine();

            if (decimal.TryParse(amount, out decimalAmount))
            {
                if (decimalAmount > decimal.MaxValue || (decimalAmount % .01m != 0) || decimalAmount < 0)
                {
                    Console.WriteLine(string.Format("Initial balance must be positive, smaller than {0}, and divisble by 0.01", decimal.MaxValue.ToString()));
                    return;
                }
            }
            else
            {
                Console.WriteLine("Initial balance is not valid");
                return;
            }

            var newTransaction = new Transaction()
            {
                Type = type,
                Amount = decimalAmount,
                accountId = guidAccountId,
                Instant = DateTime.Now
            };

            string url = localApiDomain + "bankaccount/CreateTransaction";
            var serializer = new JavaScriptSerializer();
            var data = serializer.Serialize(newTransaction);

            bool result = false;

            try
            {
                result = serializer.Deserialize<bool>(PostApiCall(url, username, password, data));
            }
            catch
            {
                Console.WriteLine("An error occured while creating transaction");
                return;
            }

            if (result)
            {
                Console.WriteLine("Successfully created transaction");
                return;
            }

            Console.WriteLine("Transaction creation failed");
        }

        public static void ViewTransactions()
        {
            Console.WriteLine();

            if (!isLoggedIn)
            {
                Console.WriteLine("Please log in\n");
                return;
            }

            Console.WriteLine("Please enter account id");
            string accountId = Console.ReadLine();
            Guid guidAccountId;

            if (string.IsNullOrWhiteSpace(accountId))
            {
                Console.WriteLine("Account Id is required");
                return;
            }

            if (!Guid.TryParse(accountId, out guidAccountId))
            {
                Console.WriteLine("Invalid account Id");
                return;
            }

            string url = localApiDomain + "bankaccount/GetTransactions?accountId=" + accountId;
            var serializer = new JavaScriptSerializer();

            var result = new List<Transaction>();

            try
            {
                result = serializer.Deserialize<List<Transaction>>(GetApiCall(url, username, password));
            }
            catch
            {
                Console.WriteLine("An error occured while fetching transactions");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Transactions");
            string resultFormat = "|{0,-20}|{1,-20}|{2,-30}|";
            Console.WriteLine(string.Format(resultFormat, "Type", "Amount", "Instant"));
            foreach (var transaction in result)
            {
                Console.WriteLine(string.Format(resultFormat, transaction.Type.ToString(), transaction.Amount.ToString(), transaction.Instant.ToString()));
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        public static string GetApiCall(string url, string apiUsername, string apiPassword)
        {
            using (var client = new WebClient())
            {
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiUsername + ":" + apiPassword));
                // Inject this string as the Authorization header
                client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

                var json = client.DownloadString(url);
                return json;
            }
        }

        public static string PostApiCall(string url, string apiUsername, string apiPassword, string data)
        {
            using (var client = new WebClient())
            {
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiUsername + ":" + apiPassword));
                // Inject this string as the Authorization header
                client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                var json = client.UploadString(url, data);
                return json;
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}