using Goldrax.Models.Authentication;

namespace Goldrax.Data
{
    public class Order
    {
        public int? Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public DateTime? DeliveryDate {  get; set; } = DateTime.Now.AddDays(5);
        public string? ShippingAddress { get; set; }

        public string? DeliveryType { get; set; } // Inside/Outside Dhaka

        public decimal? DeliveryFee { get; set; }
        public decimal? TotalAmount { get; set; }

        public string? PaymentMethod { get; set; } // "Stripe" or "CashOnDelivery"

        public string? PaymentId { get; set; } // Stripe payment ID or null if COD

        public string? PaymentStatus { get; set; } = "Pending"; // "Pending", "Paid", "Failed"

        public string? OrderStatus { get; set; } = "Processing";

        public ICollection<OrderItem>? OrderItems { get; set; }
    }


}
