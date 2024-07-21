using E_C_WebApplication1.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace E_C_WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Roles = db.Roles.ToList();
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ApplicationUser user, int? selectedRole)
        {
            if (ModelState.IsValid && selectedRole.HasValue)
            {
                // Add user role
                user.RoleId = selectedRole.Value;

                // Hash the password before saving (you may need to implement a password hashing method)
                user.Password = HashPassword(user.Password);

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            ViewBag.Roles = db.Roles.ToList();
            return View(user);
        }
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            // Hash the incoming password to compare with the stored hash
            var hashedPassword = HashPassword(password);

            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);
            if (user != null)
            {
                Session["UserRoles"] = user.Role.RoleName.ToString();
                // Set up authentication cookie
                FormsAuthentication.SetAuthCookie(username, false);
                return RedirectToAction("Index", "ProductCatalog");
            }
            ModelState.AddModelError("", "Invalid username or password");

            return View();
        }

        [Authorize]
        public ActionResult UserProfile()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Account/Profile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Address = model.Address;
                    db.SaveChanges();
                    ViewBag.Message = "Profile updated successfully.";
                }
                return View(user);
            }
            else {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
            }
            return View(model);
           
        }

        private string HashPassword(string password)
        {
            // Implement a method to hash the password, e.g., using SHA256 or another hashing algorithm
            // This is a simplified example, in practice you should use a library like BCrypt or ASP.NET Identity's PasswordHasher
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            // Clear the authentication cookie
            FormsAuthentication.SignOut();

            // Redirect to the home page or login page
            return RedirectToAction("Index", "Home");
        }
    }
}
