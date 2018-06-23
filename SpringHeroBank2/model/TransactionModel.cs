using System;
using System.Collections.Generic;
using ConsoleApp3.model;
using MySql.Data.MySqlClient;
using SpringHeroBank.entity;
using static SpringHeroBank.entity.Transaction;

namespace SpringHeroBank.model
{
    public class TransactionModel
    {
        public List<Transaction> GetTransactionListByCurrentTime()
        {
            List<Transaction> listTransactions = new List<Transaction>();
            DbConnection.Instance().OpenConnection();
            Transaction transaction;
            DateTime myDateTime = DateTime.Now;
            string currentTime = DateTime.Now.ToString("M/d/yy");
            string previoustime = DateTime.Now.AddDays(-7).ToString("M/d/yy");
            var queryString = "select * from  `transactions` where senderAccountNumber = @senderAccountNumber " +
                              "and createdAt = @createdAt and status = 2";
            
            var cmd = new MySqlCommand(queryString, DbConnection.Instance().Connection);
            cmd.Parameters.AddWithValue("@senderAccountNumber", Program.currentLoggedIn.AccountNumber);
            cmd.Parameters.AddWithValue("@createdAt", previoustime);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetString("id");
                var createdAt = reader.GetString("createdAt");
                var receiverAccountNumber = reader.GetString("receiverAccountNumber");
                var type = (TransactionType) reader.GetInt32("type");
                var amount = reader.GetDecimal("amount");
                var content = reader.GetString("content");
                transaction = new Transaction(id, createdAt, type, amount, content, receiverAccountNumber);
                listTransactions.Add(transaction);
            }
            DbConnection.Instance().CloseConnection();
            return listTransactions;
        }
    }
}