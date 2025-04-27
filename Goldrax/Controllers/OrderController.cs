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

        [HttpPost("Place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderModel order)
        {
            var result = await _orderRepository.PlaceOrderAsync(order);
            return Ok(result);
        }
    }
}
