using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_2_ADAT_Lab.Models;

namespace Week_2_ADAT_Lab
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool UpdateOrder(Order order)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                  @"UPDATE dbo.Orders
                  SET CustomerId = @CustomerId,
                  OrderDate = @OrderDate,
                  OrderStatus = @OrderStatus
              WHERE OrderId = @OrderId;", conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                    cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                    cmd.Parameters.AddWithValue("@OrderStatus", order.OrderStatus);
                    cmd.Parameters.AddWithValue("@OrderId", order.OrderId);

                    int rows = cmd.ExecuteNonQuery();
                    return rows == 1;
                }
            }
        }

        public bool DeleteOrderItem(int orderItemId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM dbo.OrderItems WHERE OrderItemId = @Id;", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", orderItemId);

                    int rows = cmd.ExecuteNonQuery();
                    return rows == 1;
                }
            }
        }

        public void UpdateOrderAndDeleteItem(Order order, int orderItemId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    SqlCommand updateOrderCmd = new SqlCommand(
                        @"UPDATE dbo.Orders
                  SET OrderStatus = @Status
                  WHERE OrderId = @OrderId;", conn, tx);

                    updateOrderCmd.Parameters.AddWithValue("@Status", order.OrderStatus);
                    updateOrderCmd.Parameters.AddWithValue("@OrderId", order.OrderId);
                    updateOrderCmd.ExecuteNonQuery();

                    SqlCommand deleteItemCmd = new SqlCommand(
                        @"DELETE FROM dbo.OrderItems
                  WHERE OrderItemId = @OrderItemId;", conn, tx);

                    deleteItemCmd.Parameters.AddWithValue("@OrderItemId", orderItemId);
                    deleteItemCmd.ExecuteNonQuery();

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
