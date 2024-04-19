using MySql.Data;
using MySql.Data.MySqlClient;


namespace BookStore
{
    public class BookStoreProgram
    {
        // Main menu
        static void main_menu(string connStr)
        {
            int userChoice;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n\n********************************************************");
            Console.WriteLine("\t  WELCOME TO THE BOOKSTORE SYSTEM");
            Console.WriteLine("********************************************************\n");
            Console.WriteLine("\t1. Top rated books list");
            Console.WriteLine("\t2. Entry of  Book Menu");
            Console.WriteLine("\t3. Buy Books");
            Console.WriteLine("\t4. Search For Book");
            Console.WriteLine("\t5. Order Menu");
            Console.WriteLine("\t6. Entry of Customer Menu");
            Console.WriteLine("\t7. Entry of Author Menu");
            Console.WriteLine("\t8. Exit");
         /*   Console.Write("Enter your choice: ");*/
         //    c = int.Parse(Console.ReadLine()); 


            // Validate the user's choice
         
            do
            {
                Console.Write("\tEnter your choice: ");
            } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice  < 1 || userChoice > 8);

            switch (userChoice)
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
                    orderMenu(connStr);
                    break;
                case 6:
                    Console.Clear();
                    customerMenu(connStr);
                    break;
                case 7:
                    Console.Clear();
                    authorMenu(connStr);
                    break;
                case 8:
                    System.Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    break;
            }
            Console.ResetColor();
        }
        // order menu
        static void orderMenu(string connStr)
        {
            int userChoice;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n\n");
            Console.WriteLine("********************************************************");
            Console.WriteLine("\t\t CUSTOMER MENU");
            Console.WriteLine("********************************************************\n");
            Console.WriteLine("\t1. Displaying the oders");
            Console.WriteLine("\t2. Place the order");
            Console.WriteLine("\t3. Cancel the order");
            Console.WriteLine("\t4. Modifying the order status");
            Console.WriteLine("\t5. Go Back");
            Console.WriteLine("\t6. Exit");
            do
            {
                Console.Write("\tEnter your choice: ");
            } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > 6);

            switch (userChoice)
            {
                case 1:
                    Console.Clear();
                    DisplayOrders(connStr);
                    break;
                case 2:
                    Console.Clear();
                    PlaceOrders(connStr);
                    break;
                case 3:
                    Console.Clear();
                    Console.Write("Enter order ID to cancel: ");
                    int orderIdToCancel = int.Parse(Console.ReadLine());
                    CancelOrder(orderIdToCancel, connStr);
                    break;
                case 4:
                    Console.Clear();
                    Console.Write("Enter order ID to modify: ");
                    int orderIdToModify = int.Parse(Console.ReadLine());
                    Console.Write("Enter new status (Pending, Processing, Shipped, Cancelled): ");
                    string newStatus = Console.ReadLine();
                    ModifyOrderStatus(orderIdToModify, newStatus, connStr);
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

        // display the orders

        static void DisplayOrders(string connStr)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "SELECT * FROM Orders";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using MySqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("Orders:");
                while (reader.Read())
                {
                    Console.WriteLine($"Order ID: {reader["order_id"]}, Customer ID: {reader["customer_id"]}, Shipper ID: {reader["shipper_id"]}, Order Date: {reader["order_date"]}, Status: {reader["order_status"]}, Total Price: {reader["total_price"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void PlaceOrders(string connStr)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                Console.Write("Enter customer ID: ");
                int customerId = int.Parse(Console.ReadLine());

                Console.Write("Enter shipper ID: ");
                int shipperId = int.Parse(Console.ReadLine());

                Console.Write("Enter total price: ");
                decimal totalPrice = decimal.Parse(Console.ReadLine());

                string insertOrderQuery = "INSERT INTO Orders (customer_id, shipper_id, order_date, order_status, total_price) " +
                                          "VALUES (@customerId, @shipperId, NOW(), 'Pending', @totalPrice)";
                MySqlCommand insertOrderCmd = new MySqlCommand(insertOrderQuery, connection);
                insertOrderCmd.Parameters.AddWithValue("@customerId", customerId);
                insertOrderCmd.Parameters.AddWithValue("@shipperId", shipperId);
                insertOrderCmd.Parameters.AddWithValue("@totalPrice", totalPrice);

                insertOrderCmd.ExecuteNonQuery();

                Console.WriteLine("Order placed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void CancelOrder(int orderId, string connStr)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string updateOrderQuery = "UPDATE Orders SET order_status = 'Cancelled' WHERE order_id = @orderId";
                MySqlCommand updateOrderCmd = new MySqlCommand(updateOrderQuery, connection);
                updateOrderCmd.Parameters.AddWithValue("@orderId", orderId);

                int affectedRows = updateOrderCmd.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    Console.WriteLine($"Order {orderId} has been cancelled.");
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

        static void ModifyOrderStatus(int orderId, string newStatus, string connStr)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string updateOrderQuery = "UPDATE Orders SET order_status = @newStatus WHERE order_id = @orderId";
                MySqlCommand updateOrderCmd = new MySqlCommand(updateOrderQuery, connection);
                updateOrderCmd.Parameters.AddWithValue("@newStatus", newStatus);
                updateOrderCmd.Parameters.AddWithValue("@orderId", orderId);

                int affectedRows = updateOrderCmd.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    Console.WriteLine($"Order {orderId} status has been updated to {newStatus}.");
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



        // author menu
        static void authorMenu(string connStr)
        {
            int userChoice;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n\n");
            Console.WriteLine("********************************************************");
            Console.WriteLine("\t\t AUTHOR MENU");
            Console.WriteLine("********************************************************\n");
            Console.WriteLine("\t1. Add new author");
            Console.WriteLine("\t2. Display all the author");
            Console.WriteLine("\t3. Update author details");
            Console.WriteLine("\t4. Delete author");
            Console.WriteLine("\t5. Go Back");
            Console.WriteLine("\t6. Exit");
            do
            {
                Console.Write("\tEnter your choice: ");
            } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > 6);

            switch (userChoice)
            {
                case 1:
                    Console.Clear();
                    AddAuthor(connStr);
                    break;
                case 2:
                    Console.Clear();
                    UpdateAuthor(connStr);
                    break;
                case 3:
                    Console.Clear();
                    DeleteAuthor(connStr);
                    break;
                case 4:
                    Console.Clear();
                    ListAuthors(connStr);
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
        // add new author
        static void AddAuthor(string connStr)
        {
            try
            {
                Console.Write("Enter first name: ");
                string firstName = Console.ReadLine();

                Console.Write("Enter last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Enter email: ");
                string email = Console.ReadLine();

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "INSERT INTO Authors (first_name, last_name, email_id) VALUES (@firstName, @lastName, @email)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Author added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // update author
        static void UpdateAuthor(string connStr)
        {
            try
            {
                Console.Write("Enter author ID to update: ");
                int authorId = int.Parse(Console.ReadLine());

                Console.Write("Enter new first name: ");
                string firstName = Console.ReadLine();

                Console.Write("Enter new last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Enter new email: ");
                string email = Console.ReadLine();

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "UPDATE Authors SET first_name = @firstName, last_name = @lastName, email_id = @email WHERE author_id = @authorId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@authorId", authorId);

                int rowsUpdated = cmd.ExecuteNonQuery();

                if (rowsUpdated > 0)
                    Console.WriteLine("Author updated successfully.");
                else
                    Console.WriteLine("Author not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // delete author
        static void DeleteAuthor(string connStr)
        {
            try
            {
                Console.Write("Enter author ID to delete: ");
                int authorId = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "DELETE FROM Authors WHERE author_id = @authorId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@authorId", authorId);

                int rowsDeleted = cmd.ExecuteNonQuery();

                if (rowsDeleted > 0)
                    Console.WriteLine("Author deleted successfully.");
                else
                    Console.WriteLine("Author not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        // display the authors
        static void ListAuthors(string connStr)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "SELECT * FROM Authors";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using MySqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("Authors:");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["author_id"]}, Name: {reader["first_name"]} {reader["last_name"]}, Email: {reader["email_id"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // customer menu
        static void customerMenu(string connStr)
        {
            int userChoice;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n\n");
            Console.WriteLine("********************************************************");
            Console.WriteLine("\t\t CUSTOMER MENU");
            Console.WriteLine("********************************************************\n");
            Console.WriteLine("\t1. Add new customer");
            Console.WriteLine("\t2. Display all the customer");
            Console.WriteLine("\t3. Update customer details");
            Console.WriteLine("\t4. Delete customer");
            Console.WriteLine("\t5. Go Back");
            Console.WriteLine("\t6. Exit");
            do
            {
                Console.Write("\tEnter your choice: ");
            } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > 6);

            switch (userChoice)
            {
                case 1:
                    Console.Clear();
                    AddCustomer(connStr);
                    break;
                case 2:
                    Console.Clear();
                    UpdateCustomer(connStr);
                    break;
                case 3:
                    Console.Clear();
                    DeleteCustomer(connStr);
                    break;
                case 4:
                    Console.Clear();
                    ListCustomers(connStr);
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

        // add the new customer
        static void AddCustomer(string connStr)
        {
            try
            {
                Console.Write("Enter first name: ");
                string firstName = Console.ReadLine();

                Console.Write("Enter last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Enter email: ");
                string email = Console.ReadLine();

                Console.Write("Enter phone number: ");
                string phoneNumber = Console.ReadLine();

                Console.Write("Enter address line 1: ");
                string addressLine1 = Console.ReadLine();

                Console.Write("Enter address line 2: ");
                string addressLine2 = Console.ReadLine();

                Console.Write("Enter city: ");
                string city = Console.ReadLine();

                Console.Write("Enter state: ");
                string state = Console.ReadLine();

                Console.Write("Enter zip code: ");
                string zipCode = Console.ReadLine();

                Console.Write("Enter country: ");
                string country = Console.ReadLine();

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "INSERT INTO Customers (first_name, last_name, email, phone_number, address_line1, address_line2, city, state, zip_code, country) " +
                               "VALUES (@firstName, @lastName, @email, @phoneNumber, @addressLine1, @addressLine2, @city, @state, @zipCode, @country)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@addressLine1", addressLine1);
                cmd.Parameters.AddWithValue("@addressLine2", addressLine2);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@state", state);
                cmd.Parameters.AddWithValue("@zipCode", zipCode);
                cmd.Parameters.AddWithValue("@country", country);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Customer added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        // update teh customer
        static void UpdateCustomer(string connStr)
        {
            try
            {
                Console.Write("Enter customer ID to update: ");
                int customerId = int.Parse(Console.ReadLine());

                Console.Write("Enter new first name: ");
                string firstName = Console.ReadLine();

                Console.Write("Enter new last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Enter new email: ");
                string email = Console.ReadLine();

                Console.Write("Enter new phone number: ");
                string phoneNumber = Console.ReadLine();

                Console.Write("Enter new address line 1: ");
                string addressLine1 = Console.ReadLine();

                Console.Write("Enter new address line 2: ");
                string addressLine2 = Console.ReadLine();

                Console.Write("Enter new city: ");
                string city = Console.ReadLine();

                Console.Write("Enter new state: ");
                string state = Console.ReadLine();

                Console.Write("Enter new zip code: ");
                string zipCode = Console.ReadLine();

                Console.Write("Enter new country: ");
                string country = Console.ReadLine();

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "UPDATE Customers SET first_name = @firstName, last_name = @lastName, email = @email, " +
                               "phone_number = @phoneNumber, address_line1 = @addressLine1, address_line2 = @addressLine2, " +
                               "city = @city, state = @state, zip_code = @zipCode, country = @country WHERE customer_id = @customerId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@addressLine1", addressLine1);
                cmd.Parameters.AddWithValue("@addressLine2", addressLine2);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@state", state);
                cmd.Parameters.AddWithValue("@zipCode", zipCode);
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                int rowsUpdated = cmd.ExecuteNonQuery();

                if (rowsUpdated > 0)
                    Console.WriteLine("Customer updated successfully.");
                else
                    Console.WriteLine("Customer not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        // delete customer
        static void DeleteCustomer(string connStr)
        {
            try
            {
                Console.Write("Enter customer ID to delete: ");
                int customerId = int.Parse(Console.ReadLine());

                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "DELETE FROM Customers WHERE customer_id = @customerId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                int rowsDeleted = cmd.ExecuteNonQuery();

                if (rowsDeleted > 0)
                    Console.WriteLine("Customer deleted successfully.");
                else
                    Console.WriteLine("Customer not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        // list the customers
        static void ListCustomers(string connStr)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connStr);
                connection.Open();

                string query = "SELECT * FROM Customers";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using MySqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("Customers:");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["customer_id"]}, Name: {reader["first_name"]} {reader["last_name"]}, " +
                                      $"Email: {reader["email"]}, Phone: {reader["phone_number"]}, " +
                                      $"Address: {reader["address_line1"]}, {reader["address_line2"]}, " +
                                      $"{reader["city"]}, {reader["state"]}, {reader["zip_code"]}, {reader["country"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
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
       // search books method
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
            int userChoice;
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
            do
            {
                Console.Write("\tEnter your choice: ");
            } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > 7);

            switch (userChoice)
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