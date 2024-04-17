using MySql.Data;
using MySql.Data.MySqlClient;


namespace BookStore
{
    public class BookStoreProgram
    {
        // Main menu
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
            Console.WriteLine("\t5. Place Order");
            Console.WriteLine("\t6. Track the Order");
            Console.WriteLine("\t7. Exit");
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
                case 3:
                    Console.Clear();
                    BuyBook(connStr);
                    break;

                case 4:
                    Console.Clear();
                    search_book(connStr);
                    break;
                case 5:
                    Console.Clear();
                    PlaceOrder(connStr);
                    break;
                case 6: 
                    Console.Clear();
                    TrackOrder(connStr);
                    break;
                case 7:
                    System.Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    break;
            }
            Console.ResetColor();
        }

        // Top rated books list
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

        // Buy books
        static void BuyBook(string connStr)
        {
            try
            {
                Console.Write("Enter book ID to buy: ");
                int bookId = int.Parse(Console.ReadLine());

                Console.Write("Enter customer ID: ");
                int customerId = int.Parse(Console.ReadLine());

                Console.Write("Enter quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "INSERT INTO Order_Details (order_id, book_id, quantity, amount) " +
                               "VALUES (@orderId, @bookId, @quantity, @amount)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@orderId", customerId); // Assuming order ID is same as customer ID
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@amount", GetBookPrice(connStr, bookId) * quantity);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Book bought successfully.");

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
        // Buy books from the search result
        static void BuyBookFromSearchQuery(string connStr,int bookId)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                // Fetch the book details
                string getBookQuery = "SELECT * FROM Books WHERE book_id = @bookId";
                MySqlCommand getBookCmd = new MySqlCommand(getBookQuery, connection);
                getBookCmd.Parameters.AddWithValue("@bookId", bookId);

                string title;
                decimal listPrice;

                using (MySqlDataReader bookReader = getBookCmd.ExecuteReader())
                {
                    if (bookReader.Read())
                    {
                        title = bookReader["title"].ToString();
                        listPrice = (decimal)bookReader["list_price"];
                        Console.WriteLine($"You have selected: {title} by Author ID: {bookReader["author_id"]}, Publisher ID: {bookReader["publisher_id"]}, Price: {listPrice}");
                    }
                    else
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }
                } // DataReader is closed here

                Console.Write("Enter customer ID: ");
                int customerId = int.Parse(Console.ReadLine());

                Console.Write("Enter quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                decimal totalPrice = listPrice * quantity;

                // Insert order details
                string insertOrderDetailsQuery = "INSERT INTO Order_Details (order_id, book_id, quantity, amount) " +
                                                 "VALUES (@customerId, @bookId, @quantity, @totalPrice)";
                MySqlCommand insertOrderDetailsCmd = new MySqlCommand(insertOrderDetailsQuery, connection);
                insertOrderDetailsCmd.Parameters.AddWithValue("@customerId", customerId);
                insertOrderDetailsCmd.Parameters.AddWithValue("@bookId", bookId);
                insertOrderDetailsCmd.Parameters.AddWithValue("@quantity", quantity);
                insertOrderDetailsCmd.Parameters.AddWithValue("@totalPrice", totalPrice);

                insertOrderDetailsCmd.ExecuteNonQuery();

                Console.WriteLine("Book bought successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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


        // Get book price
        static decimal GetBookPrice(string connStr, int bookId)
        {
            using MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();

            string query = "SELECT list_price FROM Books WHERE book_id = @bookId";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@bookId", bookId);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToDecimal(result);
            }

            return 0;
        }



        // Search for books
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
                string searchQuery = @"SELECT Books.book_id, Books.title, Authors.first_name, Authors.last_name, Publishers.name, Books.publication_date, Books.list_price 
                                     FROM Books 
                                     INNER JOIN Authors ON Books.author_id = Authors.author_id 
                                     INNER JOIN Publishers ON Books.publisher_id = Publishers.publisher_id 
                                     WHERE Books.title LIKE @keyword OR CONCAT(Authors.first_name, ' ', Authors.last_name) LIKE @keyword";

                using var cmd = new MySqlCommand(searchQuery, conn);
                cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int bookId = Convert.ToInt32(reader["book_id"]);
                    string title = reader["title"].ToString();
                    string author = $"{reader["first_name"]} {reader["last_name"]}";
                    string publisher = reader["name"].ToString();
                    DateTime publicationDate = Convert.ToDateTime(reader["publication_date"]);
                    decimal price = Convert.ToDecimal(reader["list_price"]);

                    Console.WriteLine($"Book ID: {bookId}");
                    Console.WriteLine($"Title: {title}");
                    Console.WriteLine($"Author: {author}");
                    Console.WriteLine($"Publisher: {publisher}");
                    Console.WriteLine($"Publication Date: {publicationDate.ToShortDateString()}");
                    Console.WriteLine($"Price: {price}$");
                    Console.WriteLine("-----------------------------------------");
                }
                Console.WriteLine("\n");
                Console.WriteLine("You want to buy the book? (Y/N)");
                string input = Console.ReadLine().ToLower();
                if (input == "y")
                {
                    Console.WriteLine("Enter the book ID to buy: ");
                    int bookId = int.Parse(Console.ReadLine());
                    BuyBookFromSearchQuery(connStr, bookId);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            book_menu(connStr);
        }

        // Place order
        static void PlaceOrder(string connStr)
        {
            try
            {
                Console.Write("Enter customer ID to place order: ");
                int customerId = int.Parse(Console.ReadLine());

                Console.Write("Enter shipper ID: ");
                int shipperId = int.Parse(Console.ReadLine());

                Console.Write("Enter discount ID: ");
                int discountId = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "INSERT INTO Orders (customer_id, shipper_id, discount_id, order_date, order_status, total_price) " +
                               "VALUES (@customerId, @shipperId, @discountId, @orderDate, 'Pending', @totalPrice)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@shipperId", shipperId);
                cmd.Parameters.AddWithValue("@discountId", discountId);
                cmd.Parameters.AddWithValue("@orderDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@totalPrice", GetOrderTotalPrice(connStr, customerId));

                cmd.ExecuteNonQuery();

                Console.WriteLine("Order placed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Get total price of the order
        static decimal GetOrderTotalPrice(string connStr, int customerId)
        {
            using MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();

            string query = "SELECT SUM(amount) FROM Order_Details WHERE order_id = @customerId";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@customerId", customerId);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToDecimal(result);
            }

            return 0;
        }

        // track the order
        static void TrackOrder(string connStr)
        {
            try
            {
                Console.Write("Enter order ID to track: ");
                int orderId = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "SELECT order_status FROM Orders WHERE order_id = @orderId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@orderId", orderId);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    Console.WriteLine($"Order Status: {result}");
                }
                else
                {
                    Console.WriteLine("Order not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    // Book menu
    static void book_menu(string connStr)
        {
            int c;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n\n");
            Console.WriteLine("********************************************************");
            Console.WriteLine("\t\t BOOKSTORE MENU");
            Console.WriteLine("********************************************************\n");
            Console.WriteLine("\t1. Add new book");
            Console.WriteLine("\t2. Display all the books");
            Console.WriteLine("\t3. Update books");
            Console.WriteLine("\t4. Delete books");
            Console.WriteLine("\t5. Go Back");
            Console.WriteLine("\t6. Exit");
            Console.Write("Enter your choice: ");
            c = int.Parse(Console.ReadLine());
            switch (c)
            {
                case 1:
                    Console.Clear();
                    AddBook(connStr);
                    break;
                case 2:
                    Console.Clear();
                    Displaybook(connStr);
                    break;
                case 3:
                    Console.Clear();
                    UpdateBook(connStr);
                    break;
                /*case 3:
                    Console.Clear();
                    buy_book(connStr);
                    break;*/
                case 4:
                    Console.Clear();
                    DeleteBook(connStr);
                    break;
                case 5:
                    Console.Clear();
                    main_menu(connStr);
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
        // Add new book
        static void AddBook(string connStr)
        {
            try
            {
                Console.Write("Enter title: ");
                string title = Console.ReadLine();

                Console.Write("Enter ISBN: ");
                string isbn = Console.ReadLine();

                Console.Write("Enter author ID: ");
                int authorId = int.Parse(Console.ReadLine());

                Console.Write("Enter publisher ID: ");
                int publisherId = int.Parse(Console.ReadLine());

                Console.Write("Enter publication date (YYYY-MM-DD): ");
                DateTime publicationDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter edition: ");
                string edition = Console.ReadLine();

                Console.Write("Enter page count: ");
                int pageCount = int.Parse(Console.ReadLine());

                Console.Write("Enter description: ");
                string description = Console.ReadLine();

                Console.Write("Enter list price: ");
                decimal listPrice = decimal.Parse(Console.ReadLine());

                Console.Write("Enter stock count: ");
                int inStock = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "INSERT INTO Books (title, isbn, author_id, publisher_id, publication_date, edition, page_count, description, list_price, in_stock) " +
                               "VALUES (@title, @isbn, @authorId, @publisherId, @publicationDate, @edition, @pageCount, @description, @listPrice, @inStock)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@isbn", isbn);
                cmd.Parameters.AddWithValue("@authorId", authorId);
                cmd.Parameters.AddWithValue("@publisherId", publisherId);
                cmd.Parameters.AddWithValue("@publicationDate", publicationDate);
                cmd.Parameters.AddWithValue("@edition", edition);
                cmd.Parameters.AddWithValue("@pageCount", pageCount);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@listPrice", listPrice);
                cmd.Parameters.AddWithValue("@inStock", inStock);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Book added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\n");

            Console.WriteLine("1. Go Back");
            Console.Write("Enter: ");
            int c = int.Parse(Console.ReadLine());
            if (c == 1)
            {
                Console.Clear();
                book_menu(connStr);
            }
            else
            {
                Console.WriteLine("Wrong Input");
            }
        }

        // Display all the books
        static void Displaybook(string connStr)
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

        // Update books
        static void UpdateBook(string connStr)
        {
            try
            {
                Console.Write("Enter book ID to update: ");
                int bookId = int.Parse(Console.ReadLine());

                Console.Write("Enter new title: ");
                string title = Console.ReadLine();

                Console.Write("Enter new ISBN: ");
                string isbn = Console.ReadLine();

                Console.Write("Enter new author ID: ");
                int authorId = int.Parse(Console.ReadLine());

                Console.Write("Enter new publisher ID: ");
                int publisherId = int.Parse(Console.ReadLine());

                Console.Write("Enter new publication date (YYYY-MM-DD): ");
                DateTime publicationDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter new edition: ");
                string edition = Console.ReadLine();

                Console.Write("Enter new page count: ");
                int pageCount = int.Parse(Console.ReadLine());

                Console.Write("Enter new description: ");
                string description = Console.ReadLine();

                Console.Write("Enter new list price: ");
                decimal listPrice = decimal.Parse(Console.ReadLine());

                Console.Write("Enter new stock count: ");
                int inStock = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "UPDATE Books SET title = @title, isbn = @isbn, author_id = @authorId, publisher_id = @publisherId, " +
                               "publication_date = @publicationDate, edition = @edition, page_count = @pageCount, description = @description, " +
                               "list_price = @listPrice, in_stock = @inStock WHERE book_id = @bookId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@isbn", isbn);
                cmd.Parameters.AddWithValue("@authorId", authorId);
                cmd.Parameters.AddWithValue("@publisherId", publisherId);
                cmd.Parameters.AddWithValue("@publicationDate", publicationDate);
                cmd.Parameters.AddWithValue("@edition", edition);
                cmd.Parameters.AddWithValue("@pageCount", pageCount);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@listPrice", listPrice);
                cmd.Parameters.AddWithValue("@inStock", inStock);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                int rowsUpdated = cmd.ExecuteNonQuery();

                if (rowsUpdated > 0)
                    Console.WriteLine("Book updated successfully.");
                else
                    Console.WriteLine("Book not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\n");

            Console.WriteLine("1. Go Back");
            Console.Write("Enter: ");
            int c = int.Parse(Console.ReadLine());
            if (c == 1)
            {
                Console.Clear();
                book_menu(connStr);
            }
            else
            {
                Console.WriteLine("Wrong Input");
            }
        }

        // Delete books
        static void DeleteBook(string connStr)
        {
            try
            {
                Console.Write("Enter book ID to delete: ");
                int bookId = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "DELETE FROM Books WHERE book_id = @bookId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                int rowsDeleted = cmd.ExecuteNonQuery();

                if (rowsDeleted > 0)
                    Console.WriteLine("Book deleted successfully.");
                else
                    Console.WriteLine("Book not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\n");

            Console.WriteLine("1. Go Back");
            Console.Write("Enter: ");
            int c = int.Parse(Console.ReadLine());
            if (c == 1)
            {
                Console.Clear();
                book_menu(connStr);
            }
            else
            {
                Console.WriteLine("Wrong Input");
            }
        }
      

        // Login to MySQL
        private static string MySqlLogin()
        {
            string server = "localhost";
            string user = "root";
            string database = "bookstore";
            string port = "3306";
            string password = "namxal";
            return $"server={server};user={user};database={database};port={port};password={password}";
        }


        // Main function
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