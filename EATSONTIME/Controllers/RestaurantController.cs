using EATSONTIME.DATA;
using EATSONTIME.Models;
using Microsoft.AspNetCore.Mvc;

namespace EATSONTIME.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDBContext _db;

        public RestaurantController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: /Restaurant/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Restaurant/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Restaurant model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.Restaurant_Tb.Add(model);
            await _db.SaveChangesAsync(); // New Restaurant ID is generated here

            TempData["Success"] = $"🎉 Welcome aboard! \"{model.Name}\" has been registered successfully. You can now login to add your food items.";
            return RedirectToAction("Register");
        }

        // GET: /Restaurant/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Restaurant/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string phone)
        {
            var restaurant = _db.Restaurant_Tb.FirstOrDefault(u => u.Email == email && u.Phone == phone);
            if (restaurant != null)
            {
                HttpContext.Session.SetInt32("RestaurantId", restaurant.RestaurentId);
                HttpContext.Session.SetString("RestaurantName", restaurant.Name);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Email or Phone Number";
            return View();
        }

        // GET: /Restaurant/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            var restaurantId = HttpContext.Session.GetInt32("RestaurantId");
            if (restaurantId == null)
            {
                return RedirectToAction("Login");
            }

            // Fetch categories for the dropdown
            var categories = _db.Categories_Tb.Where(c => c.RestaurentId == restaurantId).ToList();

            // If no categories exist, create a default "Food" category
            if (!categories.Any())
            {
                var defaultCategory = new Categories
                {
                    CategoryName = "Food",
                    RestaurentId = (int)restaurantId
                };
                _db.Categories_Tb.Add(defaultCategory);
                _db.SaveChanges();
                categories.Add(defaultCategory);
            }

            ViewBag.Categories = categories;

            // Fetch food items
            var foodItems = _db.FoodItems_Tb.Where(f => f.RestaurentId == restaurantId).ToList();
            return View(foodItems);
        }

        // POST: /Restaurant/AddFoodItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFoodItem(List<string> itemNames, decimal Price, int CategoryId)
        {
            var restaurantId = HttpContext.Session.GetInt32("RestaurantId");
            if (restaurantId == null) return RedirectToAction("Login");

            if (itemNames != null && itemNames.Any())
            {
                var restaurantName = HttpContext.Session.GetString("RestaurantName") ?? "Restaurant";
                foreach (var name in itemNames)
                {
                    // Clean the name (remove emojis like in registration)
                    string cleanName = System.Text.RegularExpressions.Regex.Replace(name, @"[^\u0000-\u007F]+", string.Empty).Trim();

                    var item = new FoodItems
                    {
                        Name = cleanName,
                        Description = $"Delicious {cleanName} prepared by {restaurantName}",
                        Price = Price,
                        CategoryId = CategoryId,
                        RestaurentId = (int)restaurantId
                    };
                    _db.FoodItems_Tb.Add(item);
                }
                await _db.SaveChangesAsync();
                TempData["Success"] = $"{itemNames.Count} food items added successfully!";
            }
            else
            {
                TempData["Error"] = "Please select at least one cuisine.";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: /Restaurant/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
