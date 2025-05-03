using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;
using Microsoft.EntityFrameworkCore;

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

        //get orders by userid
        //public async Task<Response<List<Order>>> GetOrdersByUserIdAsync(string userId)
        //{
        //    var orders = await _context.Orders.Where(x => x.UserId == userId).Select(order => new Order
        //    {
        //         DeliveryDate = order.DeliveryDate,
        //         OrderItems = order.OrderItems,
        //         OrderStatus = order.OrderStatus,
        //         PaymentStatus = order.PaymentStatus,

        //    }).ToListAsync();

        //    if (orders == null) return new Response<List<Order>>(false, "Orders not found");
        //    return new Response<List<Order>>(true, "Orders Lists", orders );
        //}

        public async Task<Response<List<Order>>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders?.Where(x => x.UserId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Select( order => new Order
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    DeliveryDate = order.DeliveryDate,
                    OrderStatus = order.OrderStatus,
                    PaymentStatus = order.PaymentStatus,
                    TotalAmount = order.TotalAmount,
                    OrderItems =  order.OrderItems.Select(item => new OrderItem
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Product = new Product 
                        {
                            Id = item.Product.Id,
                            Name = item.Product.Name,
                            Image = item.Product.Image,
                            // add more fields if needed
                        }
                    }).ToList()
                })
                .ToListAsync();

            if (orders == null || orders?.Count == 0)
                return new Response<List<Order>>(false, "Orders not found");

            return new Response<List<Order>>(true, "Orders list", orders);
        }

        //getAllOrder for admin
        public async Task<Response<List<Order>>> GetAllOrderAsync(string orderStatus)
        {
            

            if(orderStatus == "Completed")
            {
                var completedOrders = await _context.Orders?.Where(x => x.OrderStatus == orderStatus).Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Select(order => new Order
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    DeliveryDate = order.DeliveryDate,
                    OrderStatus = order.OrderStatus,
                    PaymentStatus = order.PaymentStatus,
                    TotalAmount = order.TotalAmount,
                    OrderItems = order.OrderItems.Select(item => new OrderItem
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Product = new Product
                        {
                            Id = item.Product.Id,
                            Name = item.Product.Name,
                            Image = item.Product.Image,
                            // add more fields if needed
                        }
                    }).ToList()
                })
                .ToListAsync();

                return new Response<List<Order>>(true, "All completed orders", completedOrders);
            }
            var orders = await _context.Orders?.Where(x => x.OrderStatus != orderStatus).Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Select(order => new Order
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    DeliveryDate = order.DeliveryDate,
                    OrderStatus = order.OrderStatus,
                    PaymentStatus = order.PaymentStatus,
                    TotalAmount = order.TotalAmount,
                    OrderItems = order.OrderItems.Select(item => new OrderItem
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Product = new Product
                        {
                            Id = item.Product.Id,
                            Name = item.Product.Name,
                            Image = item.Product.Image,
                            // add more fields if needed
                        }
                    }).ToList()
                })
                .ToListAsync();

            return new Response<List<Order>>(true, "All Current orders", orders);


        }


    }
}
