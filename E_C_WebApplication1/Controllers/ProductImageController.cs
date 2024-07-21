using E_C_WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_C_WebApplication1.Controllers
{
    public class ProductImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductImage/Create
        [AuthorizeRole("Admin", "Seller")]
        public ActionResult Create(int productId)
        {
            ViewBag.ProductId = productId;
            return View();
        }

        // POST: ProductImage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "Seller")]
        public ActionResult Create([Bind(Include = "ImageId,ImageUrl,ProductId")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                db.ProductImages.Add(productImage);
                db.SaveChanges();
                return RedirectToAction("Details", "Product", new { id = productImage.ProductId });
            }

            ViewBag.ProductId = productImage.ProductId;
            return View(productImage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}