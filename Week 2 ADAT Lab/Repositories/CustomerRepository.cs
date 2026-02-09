using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_2_ADAT_Lab.Models;

namespace Week_2_ADAT_Lab.Repositories
{
    public class CustomerRepository
    {
        private readonly string _connectionString;
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Customer> GetAll()
        {
            List<Customer> results = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT CustomerId,
                     FirstName,
                     LastName,
                     Email,
                     IsActive
                     FROM dbo.Customers;", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer c = new Customer
                            {
                                CustomerId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            };

                            results.Add(c);
                        }
                    }
                }
            }

            return results;
        }

        public Customer? GetById(int id)
        {
            Customer? result = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT CustomerId,
                     FirstName,
                     LastName,
                     Email,
                     IsActive
                     FROM dbo.Customers
                     WHERE CustomerId = @Id;", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new Customer
                            {
                                CustomerId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            };
                        }
                    }
                }
            }

            return result;
        }
    }
}
