
using MySql.Data.MySqlClient;

namespace BookStore
{
    public class BookStoreProgram
    {
        static void main_menu()
        {
            int c;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("\n\n");
            Console.WriteLine("********************************************************");
            Console.WriteLine("\t  WELCOME TO THE BOOKSTORE SYSTEM");
            Console.WriteLine("********************************************************\n");
            Console.WriteLine("\t1. Top rated books list");
            Console.WriteLine("\t2. Entry of New Book");
            Console.WriteLine("\t3. Buy Books");
            Console.WriteLine("\t4. Search For Book");
            Console.WriteLine("\t5. Edit Details of Book");
            Console.WriteLine("\t6. Exit");
            Console.Write("Enter your choice: ");
            c = int.Parse(Console.ReadLine());
            switch (c)
            {
                

                case 6:
                    System.Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    break;
            }
            Console.ResetColor();
        }

        static void book_menu()
    {
        int c;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("\n\n");
        Console.WriteLine("********************************************************");
        Console.WriteLine("\t\t BOOKSTORE MENU");
        Console.WriteLine("********************************************************\n");
        Console.WriteLine("\t1. Add");
        Console.WriteLine("\t2. Display");
        Console.WriteLine("\t3. Buy Books");
        Console.WriteLine("\t4. Go Back");
        Console.WriteLine("\t5. Exit");
        Console.Write("Enter your choice: ");
        c = int.Parse(Console.ReadLine());
        switch (c)
        {
            case 2:
                Console.Clear();
                display_book();
                break;
               case 4:
                Console.Clear();
                main_menu();
                break;
            case 5:
                System.Environment.Exit(1);
                break;
            default:
                Console.WriteLine("Wrong Input");
                break;
        }
        Console.ResetColor();
    }

static void display_book()
    {
       Console.WriteLine("Displaying book");
    }

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            string connStr = "server=localhost;user=root;database=bookstore;port=3306;password=namxal";
            MySqlConnection conn = new MySqlConnection(connStr);

            while (true)
            {
                main_menu();
            }
        }
    }
}