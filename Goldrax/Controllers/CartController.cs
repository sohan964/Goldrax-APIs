using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Repositories.CartRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Goldrax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ApplicationDbContext _context;

        public CartController(ICartRepository cartRepository, ApplicationDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

        [HttpPost("addtocart")]
        public async Task<IActionResult> AddToCart([FromBody] CartModel cart)
        {

            //here I want to add if the cart.UserId and cart.ProductId is name then I want to call the quantity update function
            var existing = await _context.Carts
                .FirstOrDefaultAsync(c =>c.UserId == cart.UserId && c.ProductId ==cart.ProductId);
            if (existing != null) { 
                   var quantity = existing.Quantity + cart.Quantity;
                    var updateCart = await _cartRepository.UpdateCartQuantityAsync(existing.Id, quantity);
                if(!updateCart.Succeeded) return Unauthorized(updateCart);
                return Ok(updateCart);
            }

            var result = await _cartRepository.AddToCartAsync(cart);
            if (!result.Succeeded) {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
