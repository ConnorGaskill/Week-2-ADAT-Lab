using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_2_ADAT_Lab.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate {  get; set; }
        public string OrderStatus {  get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public void AddItem(OrderItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            item.OrderId = OrderId;

            if (item.UnitPrice <= 0 && item.Product != null)
                item.UnitPrice = item.Product.UnitPrice;

            OrderItems.Add(item);
        }

        public void RemoveItem(int orderItemId)
        {
            var item = OrderItems.FirstOrDefault(i => i.OrderItemId == orderItemId);

            if (item == null)
                throw new Exception($"Order item {orderItemId} not found.");

            OrderItems.Remove(item);
        }

        public void PrintOrderItems()
        {
            if (OrderItems == null || OrderItems.Count == 0)
            {
                Console.WriteLine($"Order {OrderId} has no items.");
                return;
            }

            Console.WriteLine($"Order {OrderId} Items:");
            Console.WriteLine("-------------------------------------------------");

            foreach (var item in OrderItems)
            {
                Console.WriteLine(
                    $"ItemId: {item.OrderItemId} | " +
                    $"ProductId: {item.Product.ProductId} | " +
                    $"Qty: {item.Quantity} | " +
                    $"Unit Price: {item.UnitPrice:C} | " +
                    $"Total Price: ${item.Total}"
                );
            }

            Console.WriteLine();
        }
    }
}
