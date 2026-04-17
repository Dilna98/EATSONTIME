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

            // Save Cuisine Types to FoodItems_Tb as detail
            if (model.CuisineTypes != null && model.CuisineTypes.Any())
            {
                // 1. Create a default category for the restaurant's initial cuisines
                var defaultCategory = new Categories
                {
                    CategoryName = "Signature Cuisines",
                    RestaurentId = model.RestaurentId
                };
                _db.Categories_Tb.Add(defaultCategory);
                await _db.SaveChangesAsync();

                // 2. Add each cuisine as a food item
                foreach (var cuisine in model.CuisineTypes)
                {
                    // Remove emojis and leading/trailing whitespace
                    string cleanName = System.Text.RegularExpressions.Regex.Replace(cuisine, @"[^\u0000-\u007F]+", string.Empty).Trim();

                    var foodItem = new FoodItems
                    {
                        Name = cleanName,
                        Description = $"Delicious {cleanName} prepared by {model.Name}",
                        Price = 0, // Default price
                        CategoryId = defaultCategory.CategoryId,
                        RestaurentId = model.RestaurentId
                    };
                    _db.FoodItems_Tb.Add(foodItem);
                }
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = $"🎉 Welcome aboard! \"{model.Name}\" has been registered successfully. Your signature cuisines have been added!";
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

            var foodItems = _db.FoodItems_Tb.Where(f => f.RestaurentId == restaurantId).ToList();
            return View(foodItems);
        }

        // GET: /Restaurant/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
