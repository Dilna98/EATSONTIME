using System.ComponentModel.DataAnnotations;

namespace EATSONTIME.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
    }
}
