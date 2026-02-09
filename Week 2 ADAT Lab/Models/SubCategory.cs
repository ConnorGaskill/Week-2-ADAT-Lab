using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week_2_ADAT_Lab.Models
{
    public class SubCategory
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
