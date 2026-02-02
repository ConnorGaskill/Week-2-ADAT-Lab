using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_2_ADAT_Lab.Models
{
    public class OrderItem
    {
        public int OrderItemId {  get; set; }
        public int OrderId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice {  get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
