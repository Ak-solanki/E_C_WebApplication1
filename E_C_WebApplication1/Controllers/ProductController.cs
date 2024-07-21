using E_C_WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace E_C_WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        [AuthorizeRole("Seller", "Admin")]
        public ActionResult Index()
        {
            var products = db.Products.ToList();
            return View(products);
        }

        // GET: Product/Create
        [AuthorizeRole("Seller", "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [AuthorizeRole("Seller", "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                if (user != null)
                {
                    product.SellerId = user.UserId;
                    db.Products.Add(product);
                    db.SaveChanges();
                    
                    if (files != null && files.Length > 0)
                    {
                        foreach (var file in files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                                file.SaveAs(path);

                                var productImage = new ProductImage
                                {
                                    ProductId = product.ProductId,
                                    ImageUrl = "/Images/" + fileName
                                };
                                db.ProductImages.Add(productImage);
                            }
                        }
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }
            return View(product);
        }

        // GET: Product/Edit/5
        [AuthorizeRole("Seller", "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Load product including related images
            Product product = db.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Pass the product directly to the view
            ViewBag.ExistingImages = product.Images.ToList(); // Pass existing images to view
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [AuthorizeRole("Seller", "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase[] files, int[] existingImageIdsToDelete)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                if (files != null && files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                            file.SaveAs(path);

                            var productImage = new ProductImage
                            {
                                ProductId = product.ProductId,
                                ImageUrl = "/Images/" + fileName
                            };
                            db.ProductImages.Add(productImage);
                        }
                    }
                    db.SaveChanges();
                }

                if (existingImageIdsToDelete != null)
                {
                    var imagesToDelete = db.ProductImages.Where(img => existingImageIdsToDelete.Contains(img.ImageId)).ToList();
                    db.ProductImages.RemoveRange(imagesToDelete);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            ViewBag.ExistingImages = db.ProductImages.Where(img => img.ProductId == product.ProductId).ToList();
            return View(product);
        }

        // GET: Product/Delete/5
        [AuthorizeRole("Seller", "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRole("Seller", "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
