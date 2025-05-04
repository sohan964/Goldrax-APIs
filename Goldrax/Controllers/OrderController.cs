using Goldrax.Models;
using Goldrax.Repositories.OrderRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Goldrax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> MakePayment([FromBody] float OrderAmount)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(OrderAmount*100),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });
           return Ok(new {clientSecret = paymentIntent.ClientSecret});
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderModel order)
        {
            var result = await _orderRepository.PlaceOrderAsync(order);
            if(!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("myorders/{userId}")]
        public async Task<IActionResult> GetOrderByUserId([FromRoute]string userId)
        {
            var result = await _orderRepository.GetOrdersByUserIdAsync(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("allorders/{orderStatus}")]
        public async Task<IActionResult> GetAllOrders([FromRoute] string orderStatus)
        {
            var result  = await _orderRepository.GetAllOrderAsync(orderStatus);
            if (!result.Succeeded) return NotFound(result);
            return Ok(result);
        }

        [HttpPatch("changestatus/{orderId}/{newStatus}")]
        public async Task<IActionResult> OrderStatusChange(int orderId, string newStatus)
        {
            var result = await _orderRepository.ChangeOrderStatusAsync(orderId, newStatus);
            if (!result.Succeeded) return BadRequest(newStatus);
            return Ok(result);
        }
    }
}
