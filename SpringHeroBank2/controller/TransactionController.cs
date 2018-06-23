using System;
using System.IO;
using SpringHeroBank.model;

namespace SpringHeroBank.controller
{
    public class TransactionController
    {
        private static TransactionModel model = new TransactionModel();
        
        public void printListTransactions()
        {
            AccountModel accountModel = new AccountModel();
            var list = model.GetTransactionListByCurrentTime();
            Console.WriteLine("{0,35}|{1,15}|{2,15}|{3,15}|{4,15}|{5,15}",
                "Transaction ID", "Date", "Transaction Type", "Receiver", "Amount", "Content");
            string[] text = { "Transaction ID\t\t\tDate\t\t\tTransaction Type\t\t\tReceiver\t\t\tAmount\t\t\tContent" };
            File.WriteAllLines("transaction.txt", text);
            
            foreach (var transaction in list)
            {
                var receiverAccount = accountModel.GetAccountByAccountNumber(transaction.ReceiverAccountNumber);
                Console.WriteLine("{0,35}|{1,15}|{2,15}|{3,15}|{4,15}|{5,15}",
                    transaction.Id, transaction.CreatedAt, transaction.Type,
                    receiverAccount.Username, transaction.Amount, transaction.Content);
                string[] lines = { transaction.Id +"\t\t\t"+ transaction.CreatedAt +"\t\t\t"+ transaction.Type.ToString()
                                   +"\t\t\t"+receiverAccount.Username +"\t\t\t"+ transaction.Amount.ToString() +"\t\t\t"+ transaction.Content };
                File.AppendAllLines("transaction.txt",lines);
            }

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}