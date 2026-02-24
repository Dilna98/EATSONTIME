using System.ComponentModel.DataAnnotations;

namespace EATSONTIME.Models
{
    public class FoodItems
    {
        [Key]
        public int FoodId { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int RestaurentId { get; set; }
    }
}
