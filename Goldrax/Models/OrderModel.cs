using Goldrax.Data;
using Goldrax.Models.Authentication;

namespace Goldrax.Models
{
    public class OrderModel
    {
        public string UserId { get; set; }
        

        
        
        public string ShippingAddress { get; set; }

        public string DeliveryType { get; set; } // Inside/Outside Dhaka

        public decimal DeliveryFee { get; set; }
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } // "Stripe" or "CashOnDelivery"

        public string? PaymentId { get; set; } // Stripe payment ID or null if COD

        public string PaymentStatus { get; set; } = "Pending"; // "Pending", "Paid", "Failed"

        public string OrderStatus { get; set; } = "Processing";

        public List<OrderItemModel> OrderItems { get; set; }
    }
}
