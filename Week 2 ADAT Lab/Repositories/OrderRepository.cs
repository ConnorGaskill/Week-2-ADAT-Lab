using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Week_2_ADAT_Lab.Models;

namespace Week_2_ADAT_Lab.Repositories
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            using SqlTransaction tx = conn.BeginTransaction();
            try
            {
                using SqlCommand orderCmd = new SqlCommand(@"
            INSERT INTO dbo.Orders (CustomerId, OrderDate, OrderStatus)
            VALUES (@CustomerId, @OrderDate, @OrderStatus);
            SELECT SCOPE_IDENTITY();", conn, tx);

                orderCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = order.CustomerId;
                orderCmd.Parameters.Add("@OrderDate", SqlDbType.DateTime2).Value = order.OrderDate;
                orderCmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 20).Value = order.OrderStatus;

                int orderId = Convert.ToInt32(orderCmd.ExecuteScalar());
                order.OrderId = orderId;

                foreach (var item in order.OrderItems)
                {
                    using SqlCommand itemCmd = new SqlCommand(@"
                INSERT INTO dbo.OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
                SELECT SCOPE_IDENTITY();", conn, tx);

                    itemCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    itemCmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.Product.ProductId;
                    itemCmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                    itemCmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = item.UnitPrice;

                    int orderItemId = Convert.ToInt32(itemCmd.ExecuteScalar());
                    item.OrderItemId = orderItemId;
                    item.OrderId = orderId;
                }

                tx.Commit();

                return orderId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }


        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            var orderLookup = new Dictionary<int, Order>();

            string sql = @"
                SELECT 
                    o.OrderId,
                    o.CustomerId,
                    o.OrderDate,
                    o.OrderStatus,
                    oi.OrderItemId,
                    oi.ProductId,
                    oi.Quantity,
                    oi.UnitPrice,
                    p.ProductName,
                    p.UnitPrice AS ProductUnitPrice,
                    p.IsActive AS ProductIsActive,
                    c.CategoryId,
                    c.CategoryName,
                    c.IsActive AS CategoryIsActive
                FROM dbo.Orders o
                LEFT JOIN dbo.OrderItems oi ON o.OrderId = oi.OrderId
                LEFT JOIN dbo.Products p ON oi.ProductId = p.ProductId
                LEFT JOIN dbo.Categories c ON p.CategoryId = c.CategoryId
                ORDER BY o.OrderId;
            ";

            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            {
                while (reader.Read())
                {
                    int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));

                    if (!orderLookup.TryGetValue(orderId, out Order order))
                    {
                        order = new Order
                        {
                            OrderId = orderId,
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                            OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus")),
                            OrderItems = new List<OrderItem>()
                        };

                        orderLookup.Add(orderId, order);
                        orders.Add(order);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("OrderItemId")))
                    {
                        var orderItem = new OrderItem
                        {
                            OrderItemId = reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                            OrderId = orderId,
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                            Product = new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                UnitPrice = reader.GetDecimal(reader.GetOrdinal("ProductUnitPrice")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("ProductIsActive")),
                                Category = new Category
                                {
                                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("CategoryIsActive"))
                                }
                            }
                        };

                        order.OrderItems.Add(orderItem);
                    }
                }
            }

            return orders;
        }

        public Order GetOrder(int orderId)
        {
            Order order = null;

            string sql = @"
        SELECT
            o.OrderId,
            o.CustomerId,
            o.OrderDate,
            o.OrderStatus,

            oi.OrderItemId,
            oi.ProductId,
            oi.Quantity,
            oi.UnitPrice,

            p.ProductName,
            p.UnitPrice AS ProductUnitPrice,
            p.IsActive AS ProductIsActive,

            c.CategoryId,
            c.CategoryName,
            c.IsActive AS CategoryIsActive
        FROM dbo.Orders o
        LEFT JOIN dbo.OrderItems oi ON o.OrderId = oi.OrderId
        LEFT JOIN dbo.Products p ON oi.ProductId = p.ProductId
        LEFT JOIN dbo.Categories c ON p.CategoryId = c.CategoryId
        WHERE o.OrderId = @OrderId
        ORDER BY oi.OrderItemId;
    ";

            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            {
                while (reader.Read())
                {
                    if (order == null)
                    {
                        order = new Order
                        {
                            OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                            OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus")),
                            OrderItems = new List<OrderItem>()
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("OrderItemId")))
                    {
                        var item = new OrderItem
                        {
                            OrderItemId = reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                            OrderId = orderId,
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                            Product = new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                UnitPrice = reader.GetDecimal(reader.GetOrdinal("ProductUnitPrice")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("ProductIsActive")),
                                Category = new Category
                                {
                                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("CategoryIsActive"))
                                }
                            }
                        };

                        order.OrderItems.Add(item);
                    }
                }
            }

            return order;
        }


        public bool UpdateOrder(Order order)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(
                @"UPDATE dbo.Orders
                  SET CustomerId = @CustomerId,
                      OrderDate = @OrderDate,
                      OrderStatus = @OrderStatus
                WHERE OrderId = @OrderId;", conn);

            cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = order.CustomerId;
            cmd.Parameters.Add("@OrderDate", SqlDbType.DateTime2).Value = order.OrderDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 20).Value = order.OrderStatus;
            cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = order.OrderId;

            conn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool DeleteOrderItem(int orderItemId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(
                "DELETE FROM dbo.OrderItems WHERE OrderItemId = @Id;", conn);

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = orderItemId;

            conn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool DeleteOrder(int orderId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(
                @"DELETE FROM dbo.Orders
            WHERE OrderId = @OrderId;", conn);

            cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

            conn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }


        public bool UpdateOrderAndDeleteItem(Order order, int orderItemId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            using SqlTransaction tx = conn.BeginTransaction();
            try
            {
                using SqlCommand updateOrderCmd = new SqlCommand(
                    @"UPDATE dbo.Orders
                    SET OrderStatus = @Status
                WHERE OrderId = @OrderId;", conn, tx);

                updateOrderCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = order.OrderStatus;
                updateOrderCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = order.OrderId;

                int orderRows = updateOrderCmd.ExecuteNonQuery();

                if (orderRows != 1)
                    throw new InvalidOperationException("Order not found.");

                using SqlCommand deleteItemCmd = new SqlCommand(
                    @"DELETE FROM dbo.OrderItems
                WHERE OrderItemId = @OrderItemId;", conn, tx);

                deleteItemCmd.Parameters.Add("@OrderItemId", SqlDbType.Int).Value = orderItemId;

                int itemRows = deleteItemCmd.ExecuteNonQuery();

                if (itemRows != 1)
                    throw new InvalidOperationException("Order item not found.");

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                return false;
            }
        }


        public void RestoreSampleOrder()
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("RestoreSampleOrder", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
