using EATSONTIME.DATA;
using EATSONTIME.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EATSONTIME.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _db;

        public HomeController(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, string? category)
        {
            // Fetch restaurants from DB
            var restaurantsQuery = _db.Restaurant_Tb.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                restaurantsQuery = restaurantsQuery.Where(r =>
                    r.Name.Contains(search) || r.Address.Contains(search));

            var restaurants = await restaurantsQuery.ToListAsync();

            // Fetch categories for the pill strip
            var categories = await _db.Categories_Tb
                .Select(c => c.CategoryName)
                .Distinct()
                .ToListAsync();

            // If no restaurants in DB yet, add sample data for visual demo
            if (!restaurants.Any())
            {
                restaurants = GetSampleRestaurants();
            }

            if (!categories.Any())
            {
                categories = new List<string>
                {
                    "🍔 Burgers", "🍕 Pizza", "🍜 Biryani",
                    "🌮 Chinese", "🥗 Salads", "🍩 Desserts",
                    "🥪 Sandwiches", "🍣 Sushi", "🥞 Breakfast"
                };
            }

            ViewBag.Restaurants = restaurants;
            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;

            return View();
        }

        public async Task<IActionResult> Orders()
        {
            // In a real app, filter by logged-in UserId from session
            var orders = await _db.Order_Tb
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Sample orders for demonstration if DB is empty
            if (!orders.Any())
            {
                orders = GetSampleOrders();
            }

            return View(orders);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ── Sample data helpers (used when DB is empty) ──────────────────────

        private static List<Restaurant> GetSampleRestaurants()
        {
            return new List<Restaurant>
            {
                new Restaurant { RestaurentId = 1, Name = "Biryani Palace",    Address = "MG Road, Bangalore",   Phone = "9876543210", Email = "biryani@palace.com"  },
                new Restaurant { RestaurentId = 2, Name = "Pizza Hub",         Address = "Koramangala, Bangalore", Phone = "9876543211", Email = "info@pizzahub.com"  },
                new Restaurant { RestaurentId = 3, Name = "Burger Nation",     Address = "Indiranagar, Bangalore", Phone = "9876543212", Email = "hello@burgernation.com"},
                new Restaurant { RestaurentId = 4, Name = "Wok & Roll",        Address = "HSR Layout, Bangalore",  Phone = "9876543213", Email = "wok@roll.com"       },
                new Restaurant { RestaurentId = 5, Name = "The Sweet Corner",  Address = "JP Nagar, Bangalore",    Phone = "9876543214", Email = "sweet@corner.com"   },
                new Restaurant { RestaurentId = 6, Name = "Spice Garden",      Address = "Whitefield, Bangalore",  Phone = "9876543215", Email = "spice@garden.com"   },
            };
        }

        private static List<Orders> GetSampleOrders()
        {
            return new List<Orders>
            {
                new Orders { OrderId = 1001, UserId = 1, OrderDate = DateTime.Now.AddDays(-1),  TotalAmount = 450,  Status = "Delivered"  },
                new Orders { OrderId = 1002, UserId = 1, OrderDate = DateTime.Now.AddHours(-3), TotalAmount = 320,  Status = "Preparing"  },
                new Orders { OrderId = 1003, UserId = 1, OrderDate = DateTime.Now.AddMinutes(-30), TotalAmount = 680, Status = "Out for Delivery" },
            };
        }
    }
}
