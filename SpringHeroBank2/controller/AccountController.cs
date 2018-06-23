using System;
using System.Xml;
using ConsoleApp3.model;
using MySql.Data.MySqlClient;
using SpringHeroBank.entity;
using SpringHeroBank.model;
using SpringHeroBank.utility;

namespace SpringHeroBank.controller
{
    public class AccountController
    {
        private AccountModel model = new AccountModel();

        public void Register()
        {
            Console.WriteLine("Please enter account information");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Confirm Password: ");
            var cpassword = Console.ReadLine();
            Console.WriteLine("Identity Card: ");
            var identityCard = Console.ReadLine();
            Console.WriteLine("Full Name: ");
            var fullName = Console.ReadLine();
            Console.WriteLine("Email: ");
            var email = Console.ReadLine();
            Console.WriteLine("Phone: ");
            var phone = Console.ReadLine();
            var account = new Account(username, password, cpassword, identityCard, phone, email, fullName);
            var errors = account.CheckValid();
            if (errors.Count == 0)
            {
                model.Save(account);
                Console.WriteLine("Register success!");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.Error.WriteLine("Please fix following errors.");
                foreach (var messagErrorsValue in errors.Values)
                {
                    Console.Error.WriteLine(messagErrorsValue);
                }
                Console.ReadLine();
            }
        }

        public bool DoLogin()
        {
            Console.WriteLine("Please enter account information");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            var account = new Account(username, password);
            
            var errors = account.ValidLoginInformation();
            if (errors.Count > 0)
            {
                Console.WriteLine("Invalid login information. Please fix the following errors.");
                foreach (var messagErrorsValue in errors.Values)
                {
                    Console.Error.WriteLine(messagErrorsValue);
                }

                Console.WriteLine("Press enter to continue...");

                Console.ReadLine();
                return false;
            }

            var acc = model.GetAccountByUserName(username);
            if (acc == null)
            {
                Console.WriteLine("Invalid login information! Please try again.");
                return false;
            }

            if (acc.Password != Hash.GenerateSaltedSHA1(password, acc.Salt))
            {
                Console.WriteLine("Invalid login information. Please try again.");
                return false;
            }

            Program.currentLoggedIn = account;
            return true;
        }

        public void Withdraw()
        {
            Console.WriteLine("Withdraw.");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter amount to withdraw: ");
            var amount = Utility.GetUnsignDecimalNumber();
            Console.WriteLine("Please enter message content: ");
            var content = Console.ReadLine();
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            var historyTransaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = Transaction.TransactionType.WITHDRAW,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedIn.AccountNumber,
                ReceiverAccountNumber = Program.currentLoggedIn.AccountNumber,
                Status = Transaction.ActiveStatus.DONE
            };
            if (model.UpdateBalance(Program.currentLoggedIn, historyTransaction))
            {
                Console.WriteLine("Transaction success!");
            }
            else
            {
                Console.WriteLine("Transaction fails, please try again!");
            }
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Current balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
        }

        public void Deposit()
        {
            Console.WriteLine("Deposit.");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter amount to deposit: ");
            var amount = Utility.GetUnsignDecimalNumber();
            Console.WriteLine("Please enter message content: ");
            var content = Console.ReadLine();
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            var historyTransaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Type = Transaction.TransactionType.DEPOSIT,
                Amount = amount,
                Content = content,
                SenderAccountNumber = Program.currentLoggedIn.AccountNumber,
                ReceiverAccountNumber = Program.currentLoggedIn.AccountNumber,
                Status = Transaction.ActiveStatus.DONE
            };
            if (model.UpdateBalance(Program.currentLoggedIn, historyTransaction))
            {
                Console.WriteLine("Transaction success!");
            }
            else
            {
                Console.WriteLine("Transaction fails, please try again!");
            }
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Current balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
        }
        

        public bool Transfer()
        {
            /*
             * Tiến hành chuyển khoản, mặc định là trong ngân hàng.
             * 1. Yêu cầu nhập số tài khoản cần chuyển.
             *     1.1. Xác minh thông tin tài khoản và hiển thị tên người cần chuyển.
             * 2. Nhập số tiền cần chuyển.
             *     2.1. Kiểm tra số dư tài khoản.
             * 3. Nhập nội dung chuyển tiền.
             *     3.1 Xác nhận nội dung chuyển tiền.
             * 4. Thực hiện chuyển tiền.
             *     4.1. Mở transaction. Mở block try catch.
             *     4.2. Trừ tiền người gửi.
             *         4.2.1. Lấy thông tin tài khoản gửi tiền một lần nữa. Đảm bảo thông tin là mới nhất.
             *         4.2.2. Kiểm tra lại một lần nữa số dư xem có đủ tiền để chuyển không.
             *             4.2.2.1. Nếu không đủ thì rollback.
             *             4.2.2.2. Nếu đủ thì trừ tiền và update vào bảng `accounts`.
             *     4.3. Cộng tiền người nhận.
             *         4.3.1. Lấy thông tin tài khoản nhận, đảm bảo tài khoản không bị khoá hoặc inactive.
             *         4.3.1.1. Nếu ok thì update số tiền cho người nhận.
             *         4.3.1.2. Nếu không ok thì rollback.
             *     4.4. Lưu lịch sử giao dịch.
             *     4.5. Kiểm tra lại trạng thái của 3 câu lệnh trên.
             *         4.5.1. Nếu cả 3 cùng thành công thì commit transaction.
             *         4.5.2. Nếu bất kỳ một câu lệnh nào bị lỗi thì rollback.
             *     4.x. Đóng, commit transaction.
             */
            
            Console.WriteLine("Transfer.");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Please enter bank account to transfer:");
            var receiverAccountNumber = Console.ReadLine();
            AccountModel model = new AccountModel();
            
            var receiverAccount = model.GetAccountByAccountNumber(receiverAccountNumber);
            var senderAccount = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            
            if (receiverAccount == null)
            {
                Console.WriteLine("Receiver account doesn't exist. Please try again.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                return false;
            }
            
            Console.WriteLine("Please enter amount to transfer: ");
            var amount = Utility.GetUnsignDecimalNumber();
            if (senderAccount.Balance < amount)
            {
                Console.WriteLine("You don't have enough money to transfer!");
                return false;
            }
            
            Console.WriteLine("Please enter message content: ");
            var content = Console.ReadLine();
            Console.WriteLine("Your message: " + content);
          
         
            Console.WriteLine("Receiver: " + receiverAccount.Username);
            Console.WriteLine("Amount: " + amount);
            Console.WriteLine("Message: " + content);
            Console.WriteLine("Do you want to transfer with the information above? (y/n)");
            var choice = Console.ReadLine();
            if (choice == "n")
            {
                return false;
            }
            
            // Thực hiện chuyển tiền
            DbConnection.Instance().OpenConnection();
            var transaction = DbConnection.Instance().Connection.BeginTransaction();

            try
            {
                var currentSenderBalance = senderAccount.Balance;
                // Update số dư vào tài khoản người gửi
                currentSenderBalance -= amount;

                // Update số dư của người gửi vào database.
                var updateSenderAccountResult = 0;
                var queryUpdateSenderAccountBalance =
                    "update `accounts` set balance = @balance where accountNumber = @accountNumber and status = 1";
                var cmdUpdateSenderAccountBalance =
                    new MySqlCommand(queryUpdateSenderAccountBalance, DbConnection.Instance().Connection);
                cmdUpdateSenderAccountBalance.Parameters.AddWithValue("@accountNumber", senderAccount.AccountNumber);
                cmdUpdateSenderAccountBalance.Parameters.AddWithValue("@balance", currentSenderBalance);
                updateSenderAccountResult = cmdUpdateSenderAccountBalance.ExecuteNonQuery();

                
                var currentReceiverBalance = receiverAccount.Balance;
                // Update số dư vào tài khoản người nhận
                currentReceiverBalance += amount;

                // Update số dư của người nhận vào database.
                var updateReceiverAccountResult = 0;
                var queryUpdateReceiverAccountBalance =
                    "update `accounts` set balance = @balance where accountNumber = @accountNumber and status = 1";
                var cmdUpdateReceiverAccountBalance =
                    new MySqlCommand(queryUpdateReceiverAccountBalance, DbConnection.Instance().Connection);
                cmdUpdateReceiverAccountBalance.Parameters.AddWithValue("@accountNumber", receiverAccount.AccountNumber);
                cmdUpdateReceiverAccountBalance.Parameters.AddWithValue("@balance", currentReceiverBalance);
                updateReceiverAccountResult = cmdUpdateReceiverAccountBalance.ExecuteNonQuery();
                
                
                // Lưu transaction log
                var insertTransactionResult = 0;
                var queryInsertTransaction = "insert into `transactions` " +
                                             "(id, type, amount, content, senderAccountNumber, receiverAccountNumber, status) " +
                                             "values (@id, @type, @amount, @content, @senderAccountNumber, @receiverAccountNumber, @status)";
                var cmdInsertTransaction =
                    new MySqlCommand(queryInsertTransaction, DbConnection.Instance().Connection);
                cmdInsertTransaction.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                cmdInsertTransaction.Parameters.AddWithValue("@type", Transaction.TransactionType.TRANSFER);
                cmdInsertTransaction.Parameters.AddWithValue("@amount", amount);
                cmdInsertTransaction.Parameters.AddWithValue("@content", content);
                cmdInsertTransaction.Parameters.AddWithValue("@senderAccountNumber",
                    senderAccount.AccountNumber);
                cmdInsertTransaction.Parameters.AddWithValue("@receiverAccountNumber",
                    receiverAccount.AccountNumber);
                cmdInsertTransaction.Parameters.AddWithValue("@status", Transaction.ActiveStatus.DONE);
                insertTransactionResult = cmdInsertTransaction.ExecuteNonQuery();
                
                
                if (updateSenderAccountResult == 1 && updateReceiverAccountResult == 1 && insertTransactionResult == 1)
                {
                    transaction.Commit();
                    Console.WriteLine("Transfer money successfully!");
                    Console.WriteLine("Your current balance: " + currentSenderBalance);
                }
            }
            catch (SpringHeroTransactionException e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                throw;
            }
            DbConnection.Instance().CloseConnection();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            return true;
        }

        public void CheckBalance()
        {
            Program.currentLoggedIn = model.GetAccountByUserName(Program.currentLoggedIn.Username);
            Console.WriteLine("Account Information");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Full name: " + Program.currentLoggedIn.FullName);
            Console.WriteLine("Account number: " + Program.currentLoggedIn.AccountNumber);
            Console.WriteLine("Balance: " + Program.currentLoggedIn.Balance);
            Console.WriteLine("Press enter to continue!");
            Console.ReadLine();
        }
    }
}