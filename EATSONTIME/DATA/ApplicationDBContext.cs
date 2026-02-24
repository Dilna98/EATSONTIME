using Microsoft.EntityFrameworkCore;
using EATSONTIME.Models;


namespace EATSONTIME.DATA
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<User> User_Tb { get; set; }
        public DbSet<Restaurant> Restaurant_Tb { get; set; }
        public DbSet<Categories> Categories_Tb { get; set; }
        public DbSet<FoodItems> FoodItems_Tb { get; set; }
        public DbSet<Orders> Order_Tb { get; set; }
        public DbSet<OrderDetails> OrderDetail_Tb { get; set; }
        public DbSet<Cart> Cart_Tb { get; set; }

    }
}
