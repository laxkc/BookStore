using MySql.Data;
using MySql.Data.MySqlClient;


namespace BookStore
{
    public class BookStoreProgram
    {
        static void main_menu(string connStr)
        {
            int c;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n\n********************************************************");
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
                case 1:
                    Console.Clear();
                    top_rated_books(connStr);
                    break;
                case 2:
                    Console.Clear();
                    book_menu(connStr);
                    break;
                case 4:
                    Console.Clear();
                    search_book(connStr);
                    break;
                case 6:
                    System.Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    break;
            }
            Console.ResetColor();
        }

        static void search_book(string connStr)
        {
            Console.WriteLine("Searching the books....\n");
            Console.Write("Please enter a keyword to search for books (e.g., title or author name):");
            string keyword = Console.ReadLine();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("\nSearch Results: \n");
                conn.Open();
                string searchQuery = @"SELECT Books.title, Authors.first_name, Authors.last_name, Publishers.name, Books.publication_date, Books.list_price 
                                     FROM Books 
                                     INNER JOIN Authors ON Books.author_id = Authors.author_id 
                                     INNER JOIN Publishers ON Books.publisher_id = Publishers.publisher_id 
                                     WHERE Books.title LIKE @keyword OR CONCAT(Authors.first_name, ' ', Authors.last_name) LIKE @keyword";

                using var cmd = new MySqlCommand(searchQuery, conn);
                cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string title = reader["title"].ToString();
                    string author = $"{reader["first_name"]} {reader["last_name"]}";
                    string publisher = reader["name"].ToString();
                    DateTime publicationDate = Convert.ToDateTime(reader["publication_date"]);
                    decimal price = Convert.ToDecimal(reader["list_price"]);

                    Console.WriteLine($"Title: {title}");
                    Console.WriteLine($"Author: {author}");
                    Console.WriteLine($"Publisher: {publisher}");
                    Console.WriteLine($"Publication Date: {publicationDate.ToShortDateString()}");
                    Console.WriteLine($"Price: {price}$");
                    Console.WriteLine("-----------------------------------------");
            }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            book_menu(connStr);
        }

        static void display_book(string connStr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                Console.WriteLine("\nAll Books in the store: \n");
                conn.Open();

                string sql = "SELECT title, description FROM  Books";
                using var cmd = new MySqlCommand(sql, conn);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetString(0)} -> {reader.GetString(1)}");
                }
                conn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("\n");
            book_menu(connStr);
        }
        static void book_menu(string connStr)
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
                    display_book(connStr);
                    break;
                case 3:
                    Console.Clear();
                    buy_book(connStr);
                    break;
                case 4:
                    Console.Clear();
                    main_menu(connStr);
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
        static void buy_book(string connStr){
            bool userAuth = true;
            if (userAuth){
                Console.WriteLine("You can buy the books!");
            } else {
                Console.WriteLine("Please login first!");
            }
        }
        static void user_auth(string connStr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            Console.Write("\t Enter  your email: ");
            var email = Console.ReadLine();
            Console.Write("\t Enter you email: ");
            var firstName = Console.ReadLine();

        }
        static void top_rated_books(string connStr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                Console.WriteLine("Top rated books...\n");
                conn.Open();

                string sql = "SELECT Books.title, AVG(Reviews.rating) as average_rating\r\nFROM Books\r\nLEFT JOIN Reviews ON Books.book_id = Reviews.book_id\r\nGROUP BY Books.title\r\nORDER BY average_rating DESC";
                using var cmd = new MySqlCommand(sql, conn);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetString(0)}   {reader.GetDecimal(1)}");
                }


                conn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("\n");

            Console.WriteLine("1. Go Back");
            Console.Write("Enter: ");
            int c = int.Parse(Console.ReadLine());
            if (c == 1)
            {
                Console.Clear();
                main_menu(connStr);
            }
            else
            {
                Console.WriteLine("Wrong Input");
            }
        }

        private static string MySqlLogin()
        {
            string server = "localhost";
            string user = "root";
            string database = "bookstore";
            string port = "3306";
            string password = "namxal";
            return $"server={server};user={user};database={database};port={port};password={password}";
        }


        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            string connStr = MySqlLogin();

            while (true)
            {
                main_menu(connStr);
            }
        }

    }
}