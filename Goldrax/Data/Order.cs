using Goldrax.Models.Authentication;

namespace Goldrax.Data
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } // FK to ApplicationUser
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string ShippingAddress { get; set; }

        public string DeliveryType { get; set; } // "Inside Dhaka" / "Outside Dhaka"

        public decimal DeliveryFee { get; set; }

        public decimal TotalAmount { get; set; } // Includes subtotal + delivery

        public string PaymentStatus { get; set; } = "Pending"; // or "Paid"

        public string OrderStatus { get; set; } = "Processing"; // Other statuses: Shipped, Delivered, Canceled

        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
