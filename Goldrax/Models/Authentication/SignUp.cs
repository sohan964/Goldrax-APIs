using System.ComponentModel.DataAnnotations;

namespace Goldrax.Models.Authentication
{
    public class SignUp
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
