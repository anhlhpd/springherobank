using System;
using SpringHeroBank.controller;
using SpringHeroBank.entity;
using SpringHeroBank.model;
using SpringHeroBank.utility;

namespace SpringHeroBank.view
{
    public class MainView
    {
        private static AccountController controller = new AccountController();

        public static void GenerateMenu()
        {
            while (true)
            {
                if (Program.currentLoggedIn == null)
                {
                    GenerateGeneralMenu();
                }
                else
                {
                    GenerateCustomerMenu();
                }
            }
        }

        private static void GenerateCustomerMenu()
        {
            while (true)
            {
                Console.WriteLine("---------SPRING HERO BANK---------");
                Console.WriteLine("Welcome back: " + Program.currentLoggedIn.FullName);
                Console.WriteLine("1. Check balance.");
                Console.WriteLine("2. Withdraw.");
                Console.WriteLine("3. Deposit.");
                Console.WriteLine("4. Transfer.");
                Console.WriteLine("5. Transaction history.");
                Console.WriteLine("6. Exit.");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Please enter your choice (1|2|3|4|5|6): ");
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        controller.CheckBalance();
                        break;
                    case 2:
                        controller.Withdraw();
                        break;
                    case 3:
                        controller.Deposit();
                        break;
                    case 4:
                        controller.Transfer();
                        break;
                    case 5:
                        GenerateTransactionMenu();
                        break;
                    case 6:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void GenerateGeneralMenu()
        {
            while (true)
            {
                Console.WriteLine("---------WELCOME TO SPRING HERO BANK---------");
                Console.WriteLine("1. Register for free.");
                Console.WriteLine("2. Login.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Please enter your choice (1|2|3): ");                
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        controller.Register();
                        break;
                    case 2:
                        if (controller.DoLogin())
                        {
                            Console.WriteLine("Login success.");
                        }

                        break;
                    case 3:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (Program.currentLoggedIn != null)
                {
                    break;
                }
            }
        }

        private static void GenerateTransactionMenu()
        {
            TransactionController transactionController = new TransactionController();
            while (true)
            {
                Console.WriteLine("---------TRANSACTION LOG---------");
                Console.WriteLine("1. Check recent transaction history (7 days).");
                Console.WriteLine("2. Find transaction by date.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Please enter your choice (1|2|3): ");                
                var choice = Utility.GetInt32Number();
                switch (choice)
                {
                    case 1:
                        transactionController.printListTransactions();
                        break;
                    case 2:
                        if (controller.DoLogin())
                        {
                            Console.WriteLine("Login success.");
                        }

                        break;
                    case 3:
                        Console.WriteLine("See you later.");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (Program.currentLoggedIn != null)
                {
                    break;
                }
            }
        }
        
    }
}