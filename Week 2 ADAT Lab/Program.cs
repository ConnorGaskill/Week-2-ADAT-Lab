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

            //ADOTests(connectionString);

            List<Order> orders = new List<Order>();

            OrderRepository orderRepo = new OrderRepository(connectionString);

            orders = orderRepo.GetAllOrders();

            foreach (Order o in orders)
                o.PrintOrderItems();


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
    }
}
