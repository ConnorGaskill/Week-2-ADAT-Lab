using System.Reflection.Metadata.Ecma335;
using Week_2_ADAT_Lab.Models;

namespace Week_2_ADAT_Lab
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "Server=(localDB)\\MSSQLLocalDB;Database=IT410Week2Demo;" +
                "Trusted_Connection=True;TrustServerCertificate=True;";

            //Console.WriteLine("Testing successful TestUpdateOrderAndDeleteItem operation...");

            //TestUpdateOrderAndDeleteItem(connectionString, true);

            //Console.WriteLine("Testing unsuccessful TestUpdateOrderAndDeleteItem operation with rollback...");

            //TestUpdateOrderAndDeleteItem(connectionString, false);

            //Console.WriteLine("Testing Successful Update operation...");

            //TestUpdate(connectionString, true);

            //Console.WriteLine("Testing Unsuccessful Update operation...");

            //TestUpdate(connectionString, false);

            Console.WriteLine("Testing Successful Delete operation...");

            TestDeleteOrderItem(connectionString, true);

            Console.WriteLine("Testing Unsuccessful Delete operation...");

            TestDeleteOrderItem(connectionString, false);



        }

        static void TestUpdate(string connectionString, bool isSuccessful)
        {
            OrderRepository repo = new OrderRepository(connectionString);

            bool success;

            Order order;

            if (isSuccessful)
            {
                order = repo.GetOrder(1);
            }
            else
            {
                order = new Order();
            }

            Console.WriteLine("--- Order before ---");

            Console.WriteLine($"Order Status: {order.OrderStatus}");
            order.PrintOrderItems();

            Console.WriteLine("Setting order status to Testing...");
            order.OrderStatus = "Testing";

            success = repo.UpdateOrder(order);

            if (success)
            {
                Console.WriteLine("--- Operation successful ---");
            }
            else
            {
                Console.WriteLine("--- Operation unsuccessful ---");
            }

            Console.WriteLine("--- Order after ---");

            Console.WriteLine($"Order Status: {order.OrderStatus}");
            order.PrintOrderItems();

            repo.DeleteOrder(1);

            repo.RestoreSampleOrder();

        }

        static void TestDeleteOrderItem(string connectionString, bool validId)
        {
            OrderRepository repo = new OrderRepository(connectionString);

            bool success;

            int id;

            Order order = repo.GetOrder(1);

            if (validId)
            {
                id = order.OrderItems[0].OrderItemId;
            }
            else
            {
                id = -1;
            }

            Console.WriteLine("Order before:");
            order.PrintOrderItems();


            Console.WriteLine("Testing Delete...");

            success = repo.DeleteOrderItem(id);

            order = repo.GetOrder(1);

            if (success)
            {
                Console.WriteLine("--- Operation successful ---");
            }
            else
            {
                Console.WriteLine("--- Operation unsuccessful ---");
            }

            Console.WriteLine("Order after:");
            order.PrintOrderItems();

            repo.DeleteOrder(1);

            repo.RestoreSampleOrder();

        }
        static void ADOTests(string connectionString)
        {
            CustomerRepository repo = new CustomerRepository(connectionString);

            Console.WriteLine("[Testing Get All]\n");

            foreach (Customer c in repo.GetAll())
                Console.WriteLine($"{c.FirstName} {c.LastName}");

            Console.WriteLine("\n[Testing getting valid ID (1)]\n");

            Customer? customer = repo.GetById(1);

            if (customer == null )
            {
                Console.WriteLine("Error: Valid customer ID returns null value");
            }
            else
            {
                Console.WriteLine($"Customer found: ID: {customer.CustomerId} {customer.FirstName} {customer.LastName}");
            }

            Console.WriteLine("\n[Testing getting invalid ID (-1)]\n");

            customer = repo.GetById(-1);

            if (customer == null)
            {
                Console.WriteLine("Invalid customer ID returns null value");
            }
            else
            {
                Console.WriteLine($" Error: Customer found: ID: {customer.CustomerId} {customer.FirstName} {customer.LastName}");
            }
        }

        static void OrderRepoTests(string connectionString)
        {
            OrderRepository repo = new OrderRepository(connectionString);

            var orders = repo.GetAllOrders();

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            var order = orders.FirstOrDefault(o => o.OrderItems.Count > 0);

            if (order == null)
            {
                Console.WriteLine("No orders with items to test.");
                return;
            }

            var orderItem = order.OrderItems.First();

            Console.WriteLine("BEFORE:");
            Console.WriteLine($"Order Status: {order.OrderStatus}");
            order.PrintOrderItems();

            order.OrderStatus = "Cancelled";

            repo.UpdateOrderAndDeleteItem(order, orderItem.OrderItemId);

            var updatedOrder = repo
                .GetAllOrders()
                .First(o => o.OrderId == order.OrderId);

            Console.WriteLine("AFTER:");
            Console.WriteLine($"Order Status: {updatedOrder.OrderStatus}");
            updatedOrder.PrintOrderItems();

            Console.WriteLine("Test complete.");

        }

        static void TestUpdateOrderAndDeleteItem(string connectionString, bool validId) {

            OrderRepository repo = new OrderRepository(connectionString);

            bool success;

            int id;

            Order order = repo.GetOrder(1);

            if (validId)
            {
                id = order.OrderItems[0].OrderItemId;
            }
            else
            {
                id = -1;
            }

            Console.WriteLine("Order before:");

            Console.WriteLine($"Order Status: {order.OrderStatus}");
            order.PrintOrderItems();

            order.OrderStatus = "Updated";

            success = repo.UpdateOrderAndDeleteItem(order, id);

            order = repo.GetOrder(1);

            if (success) {
                Console.WriteLine("--- Operation successful ---");
            }
            else
            {
                Console.WriteLine("--- Operation unsuccessful ---");
            }

            Console.WriteLine("Order after:");

            Console.WriteLine($"Order Status: {order.OrderStatus}");
            order.PrintOrderItems();

            repo.DeleteOrder(1);

            repo.RestoreSampleOrder();

        }
    }
}
