namespace Week_2_ADAT_Lab
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "Server=(localDB)\\MSSQLLocalDB;Database=IT410Week2Demo;" +
                "Trusted_Connection=True;TrustServerCertificate=True;";

            ADOTests(connectionString);
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
    }
}
