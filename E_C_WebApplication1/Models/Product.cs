using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_C_WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string Category { get; set; }

        public int StockQuantity { get; set; }

        public int SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual ApplicationUser Seller { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }


    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        public string ImageUrl { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
    public class Review
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } // Foreign key to ApplicationUser (AspNetUsers)
        public string Content { get; set; }
        public int Rating { get; set; } // Rating out of 5
        public DateTime CreatedAt { get; set; }

        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; } // Assuming ApplicationUser is your user model
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }

}