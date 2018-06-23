using System;
using System.Collections.Generic;
using System.IO;
using SpringHeroBank.entity;

namespace SpringHeroBank.utility
{
    public class FileHandle
    {
        public static List<Transaction> ReadTransactions()
        {
            var list = new List<Transaction>();
            var lines = File.ReadAllLines("NeverEverGetBackTogether.txt");
            for (var i = 0; i < lines.Length; i += 1)
            {
                if (i == 0)
                {
                    continue;
                }

                var linesSplited = lines[i].Split("|");
                if (linesSplited.Length == 8)
                {
                    
//                    if (i > 0)
//                {
//                    var line = lines[i];
//                    string[] stringSeparators = new string[] {"|"};
//                    string[] result;
//            
//                    result = line.Split(stringSeparators, 
//                        StringSplitOptions.RemoveEmptyEntries);
//                    Console.WriteLine("Result including non-empty elements ({0} elements):", 
//                        result.Length);
//                    Console.Write("   ");
//                    string id = result[0].Trim();
//                    string senderAccountNumber = result[1].Trim();
//                    string receiverAccountNumber = result[2].Trim();
//                    string strType = result[3].Trim();
//                    var type = Enum.Parse<Transaction.TransactionType>(strType);
//                    string strAmount = result[4].Trim();
//                    var amount = Decimal.Parse(strAmount);
//                    string content = result[5].Trim();
//                    string createdAt = result[6].Trim();
//                    string strStatus = result[7].Trim();
//                    var status = Enum.Parse<Transaction.ActiveStatus>(strStatus);
//                
//                    Console.WriteLine();
//                
//                    list.Add(new Transaction(id,createdAt, type, amount, content,
//                        senderAccountNumber, receiverAccountNumber, status));  
//                }
                    
                    var tx = new Transaction()
                    {
                        Id = linesSplited[0],
                        SenderAccountNumber = linesSplited[1],
                        ReceiverAccountNumber = linesSplited[2],
                        Type = (Transaction.TransactionType) Int32.Parse(linesSplited[3]),
                        Amount = Decimal.Parse(linesSplited[4]),
                        Content = linesSplited[5],
                        CreatedAt = linesSplited[6],
                        Status = (Transaction.ActiveStatus) Int32.Parse(linesSplited[7])
                    };
                    list.Add(tx);
                }
            }

            return list;
        }

        public static Dictionary<string, Account> ReadAccounts()
        {
            var dictionary = new Dictionary<string, Account>();
            var lines = File.ReadAllLines("ForgetMeNot.txt");
            for (var i = 0; i < lines.Length; i += 1)
            {
                if (i == 0)
                {
                    continue;
                }

                var linesSplited = lines[i].Split("|");
                if (linesSplited.Length == 6)
                {
                    var acc = new Account()
                    {
                        AccountNumber = linesSplited[0],
                        Username = linesSplited[1],
                        FullName = linesSplited[2],
                        Balance = Decimal.Parse(linesSplited[3]),
                        Salt = linesSplited[4],
                        Status = (Account.ActiveStatus) Int32.Parse(linesSplited[5])
                    };
                    if (dictionary.ContainsKey(acc.AccountNumber))
                    {
                        continue;
                    }

                    dictionary.Add(acc.AccountNumber, acc);
                }
            }

            return dictionary;
        }
    }
}