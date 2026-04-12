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
            await _db.SaveChangesAsync();

            TempData["Success"] = $"🎉 Welcome aboard! \"{model.Name}\" has been registered successfully.";
            return RedirectToAction("Register");
        }
    }
}
