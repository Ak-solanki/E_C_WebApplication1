using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_C_WebApplication1.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<ProductImage> Images { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
    }
}