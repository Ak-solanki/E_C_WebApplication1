using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_C_WebApplication1.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public string FirstImageUrl { get; set; }
    }
}