using System.ComponentModel.DataAnnotations;

namespace EATSONTIME.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int RestaurentId { get; set; }

    }
}
