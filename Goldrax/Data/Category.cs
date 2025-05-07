using System.ComponentModel.DataAnnotations;

namespace Goldrax.Data
{
    public class Category
    {
        public int? Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Subcategory>? Subcategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
