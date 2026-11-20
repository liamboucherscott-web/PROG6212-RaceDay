using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public string? PhoneNumber { get; set; }

       
        [Required]
        public int RoleId { get; set; }
    }
}
