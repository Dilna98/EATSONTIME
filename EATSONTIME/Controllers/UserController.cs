using EATSONTIME.DATA;
using EATSONTIME.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EATSONTIME.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _db;

        public UserController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if mobile or email already exists
                var exists = await _db.User_Tb.AnyAsync(u => u.Mobile == model.Mobile || u.Email == model.Email);
                if (exists)
                {
                    ViewBag.Error = "A user with this mobile or email already exists.";
                    return View(model);
                }

                _db.User_Tb.Add(model);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Account created successfully! Please login.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                ViewBag.Error = "Please enter your mobile number.";
                return View();
            }

            var user = await _db.User_Tb.FirstOrDefaultAsync(u => u.Mobile == mobile);

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.Name);
                
                // Note: Role is not in the current User_Tb schema, so we skip setting it or use a default
                HttpContext.Session.SetString("UserRole", "Customer"); 

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid mobile number.";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
