using Goldrax.Data;
using Goldrax.Models.Authentication;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Goldrax.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }
        public string? Discount { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string SellerId { get; set; }
        public int SubcategoryId { get; set; }
        

        public object? Category { get; set; }
        public object? Subcategory { get; set; }
        public object? Seller { get; set; }
    }
}
