﻿namespace Goldrax.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }
        

        public int Quantity { get; set; }

        public decimal Price { get; set; } // Price at the time of order
    }
}
