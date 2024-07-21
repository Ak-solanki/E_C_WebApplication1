using E_C_WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace E_C_WebApplication1.Controllers
{
    public class ProductCatalogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductCatalog
        public ActionResult Index(string searchQuery = "", string selectedCategory = "", decimal minPrice = 0, decimal maxPrice = 1000, int? minRating = null, int? maxRating = null, int page = 1)
        {
            const int pageSize = 10;

            // Join Products with ProductImages and Reviews
            var productsQuery = from p in db.Products
                                join pi in db.ProductImages on p.ProductId equals pi.ProductId into images
                                from pi in images.DefaultIfEmpty()
                                join r in db.Reviews on p.ProductId equals r.ProductId into reviews
                                from r in reviews.DefaultIfEmpty()
                                select new
                                {
                                    p.ProductId,
                                    p.Name,
                                    p.Price,
                                    p.Description,
                                    p.Category,
                                    p.StockQuantity,
                                    ImageUrl = pi.ImageUrl,
                                    ReviewRating = (double?)r.Rating
                                };

            // Apply search filters
            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                productsQuery = productsQuery.Where(p => p.Category == selectedCategory);
            }

            if (minPrice >= 0 && maxPrice > 0)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            }

            if (minRating.HasValue && maxRating.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ReviewRating >= minRating.Value && p.ReviewRating <= maxRating.Value);
            }

            // Group by product to eliminate duplicate rows caused by the joins
            var groupedProductsQuery = from p in productsQuery
                                       group p by new
                                       {
                                           p.ProductId,
                                           p.Name,
                                           p.Price,
                                           p.Description,
                                           p.Category,
                                           p.StockQuantity,
                                           p.ImageUrl
                                       } into grouped
                                       select new
                                       {
                                           grouped.Key.ProductId,
                                           grouped.Key.Name,
                                           grouped.Key.Price,
                                           grouped.Key.Description,
                                           grouped.Key.Category,
                                           grouped.Key.StockQuantity,
                                           ImageUrls = grouped.Select(g => g.ImageUrl).Distinct(),
                                           AverageRating = grouped.Average(g => g.ReviewRating ?? 0) // Handle null ratings
                                       };

            var totalProducts = groupedProductsQuery.Count();
            var products = groupedProductsQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var categories = db.Categories.ToList();

            var viewModel = new ProductFilterViewModel
            {
                Products = products.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Category = p.Category,
                    StockQuantity = p.StockQuantity,
                    Images = p.ImageUrls.Select(url => new ProductImage { ImageUrl = url }).ToList(),
                    Reviews = p.AverageRating > 0 ? new List<Review> { new Review { Rating = (int)p.AverageRating } } : new List<Review>()
                }).ToList(),
                Categories = categories,
                SelectedCategory = selectedCategory,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinRating = minRating,
                MaxRating = maxRating,
                SearchQuery = searchQuery,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
            };

            return View(viewModel);
        }


        public ActionResult Details(int id)
        {
            var product = db.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    Product = p,
                    Images = p.Images,
                    Reviews = p.Reviews
                })
                .SingleOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ProductDetailsViewModel
            {
                Product = product.Product,
                Images = product.Images.ToList(),
                Reviews = product.Reviews.ToList(),
                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0
            };

            return View(viewModel);
        }


    }

}