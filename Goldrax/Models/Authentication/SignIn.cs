using System.ComponentModel.DataAnnotations;

namespace Goldrax.Models.Authentication
{
    public class SignIn
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
