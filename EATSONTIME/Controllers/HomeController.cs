using EATSONTIME.DATA;
using EATSONTIME.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> Index(string? search, string? category, string? dish)
        {
            // Curated list of dishes for the "What's on your mind?" carousel (matching our premium images)
            var curatedDishes = new List<string> { "Biryani", "Pizza", "Dosa", "Noodles", "Shawarma", "Cake", "Juice" };
            
            // Fetch distinct food items from DB to see if any others should be added
            var dbFoodItems = await _db.FoodItems_Tb
                .Select(f => f.Name)
                .Distinct()
                .ToListAsync();

            // Combine curated with DB items, keeping curated first
            var carouselDishes = curatedDishes.Union(dbFoodItems).ToList();

            // Fetch restaurants from DB
            var restaurantsQuery = _db.Restaurant_Tb.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                restaurantsQuery = restaurantsQuery.Where(r =>
                    r.Name.Contains(search) || r.Address.Contains(search));
            }
            
            if (!string.IsNullOrWhiteSpace(dish))
            {
                // Find restaurants that serve this specific dish in their FoodItems_Tb
                var restaurantIdsWithDish = await _db.FoodItems_Tb
                    .Where(f => f.Name == dish || f.Name.Contains(dish))
                    .Select(f => f.RestaurentId)
                    .Distinct()
                    .ToListAsync();
                
                restaurantsQuery = restaurantsQuery.Where(r => restaurantIdsWithDish.Contains(r.RestaurentId));
            }

            var restaurants = await restaurantsQuery.ToListAsync();

            // Fetch categories for the pill strip
            var categories = await _db.Categories_Tb
                .Select(c => c.CategoryName)
                .Distinct()
                .ToListAsync();

            // If no restaurants in DB yet, add sample data for visual demo (except when filtering)
            if (!restaurants.Any() && string.IsNullOrEmpty(dish) && string.IsNullOrEmpty(search))
            {
                restaurants = GetSampleRestaurants();
            }

            ViewBag.Restaurants = restaurants;
            ViewBag.Categories = categories;
            ViewBag.FoodItems = carouselDishes;
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedDish = dish;

            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Fetch orders for the logged-in user
            var orders = await _db.Order_Tb
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderViewModels = new List<OrderHistoryViewModel>();

            foreach (var order in orders)
            {
                // Get details for this order
                var details = await (from od in _db.OrderDetail_Tb
                                     join f in _db.FoodItems_Tb on od.FoodId equals f.FoodId
                                     join r in _db.Restaurant_Tb on f.RestaurentId equals r.RestaurentId
                                     where od.OrderId == order.OrderId
                                     select new
                                     {
                                         f.Name,
                                         od.Quantity,
                                         od.Price,
                                         RestaurantName = r.Name
                                     }).ToListAsync();

                var vm = new OrderHistoryViewModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    RestaurantName = details.FirstOrDefault()?.RestaurantName ?? "Unknown Restaurant",
                    ItemsSummary = string.Join(", ", details.Select(d => $"{d.Name} × {d.Quantity}")),
                    Items = details.Select(d => new OrderItemDetail
                    {
                        FoodName = d.Name,
                        Quantity = d.Quantity,
                        Price = d.Price
                    }).ToList()
                };

                orderViewModels.Add(vm);
            }

            return View(orderViewModels);
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
