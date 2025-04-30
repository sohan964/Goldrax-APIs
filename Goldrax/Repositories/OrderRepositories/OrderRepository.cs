using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.OrderRepositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //store payment
        public async Task<Response<object>> PlaceOrderAsync(OrderModel order)
        {
            var newOrder = new Order()
            {
                OrderDate = DateTime.UtcNow,
                UserId = order.UserId,
                ShippingAddress = order.ShippingAddress,
                DeliveryType = order.DeliveryType,
                DeliveryFee = order.DeliveryFee,
                DeliveryDate = DateTime.UtcNow.AddDays(5),
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                PaymentId = (order.PaymentId),
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
            };
            var result = await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
            var newOrderItems = new List<OrderItem>();
            
            order?.OrderItems?.ForEach(item =>
            {
                var newItem = new OrderItem()
                {
                    OrderId = newOrder.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                };
                newOrderItems.Add(newItem);
            });

            Console.WriteLine(newOrderItems);

            await _context.OrderItems.AddRangeAsync(newOrderItems);
            var res = await _context.SaveChangesAsync();
            if(res == 0)
            {
                return new Response<object>(false, "hoynai");
            }
            return new Response<object>(true, "hoice");


        }
    }
}
