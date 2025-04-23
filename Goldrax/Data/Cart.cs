using Goldrax.Models.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goldrax.Data
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId {  get; set; }

        [Required]
        public int? ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Quantity must be at least 1")]
        public int? Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; } // If using Identity

        public virtual Product? Product { get; set; }

    }
}
