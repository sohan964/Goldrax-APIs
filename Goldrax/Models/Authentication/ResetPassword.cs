using System.ComponentModel.DataAnnotations;

namespace Goldrax.Models.Authentication
{
    public class ResetPassword
    {
        [Required]
        public string? Password { get; set; }

        [Compare("Password")]
        public string? ConfirmedPassword { get; set; }

        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
