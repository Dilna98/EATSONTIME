using System.ComponentModel.DataAnnotations;

namespace EATSONTIME.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "passwored must be between 6 and 20 letters")]
        public string Password { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Address { get; set; }


    }
}
