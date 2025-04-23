using System.ComponentModel.DataAnnotations;

namespace Goldrax.Models
{
    public class CartModel
    {

        [Required]
        public string? UserId { get; set; }

        [Required]
        public int? ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int? Quantity { get; set; }
    }
}
