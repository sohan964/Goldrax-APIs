using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Goldrax.Repositories.CartRepositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //add to cart
        public async Task<Response<object>> AddToCartAsync(CartModel cart)
        {
            var newCart = new Cart()
            {
                ProductId = cart.ProductId,
                UserId = cart.UserId,
                Quantity = cart.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            await _context.Carts.AddAsync(newCart);
            var res = await _context.SaveChangesAsync();
            if(res == 0) return new Response<object>(false, "Fail to add to cart");
            return new Response<object>(true,"asdf", res);
        }

        //update cart quantity
        public async Task<Response<object>> UpdateCartQuantityAsync(int id, int? quantity)
        {
            var exCart = await _context.Carts.FindAsync(id);
            if (exCart == null) return new Response<object>(false, "Cart not found");
            exCart.Quantity = quantity;
            exCart.UpdatedAt = DateTime.UtcNow;
            var res = await _context.SaveChangesAsync();
            if (res == 0)  return new Response<object>(false, "Quantity Update Fail");
            return new Response<object>(true, "Quantity Updated success");
        }

        public async Task<Response<object>> DeleteByIdAsync( int id)
        {
            var deleteCart = await _context.Carts.FindAsync(id);
            if(deleteCart == null) return new Response<object> ( false, "Cart not found " );

             _context.Carts.Remove(deleteCart);
            var res = await _context.SaveChangesAsync();
            if (res == 0) return new Response<object>(false, "Cart Delete faild");
            return new Response<object>(true, "Cart delete Success");
        }

        public async Task<Response<object>> DeleteByUserIdAsync(string userId)
        {
            var userCarts = await _context.Carts.Where(x => x.UserId == userId).ToListAsync();
            if (userCarts == null) return new Response<object>(false, "Carts not found");
            _context.Carts.RemoveRange(userCarts);
            await _context.SaveChangesAsync();
            return new Response<object>(true, "Remove carts Success");
        }

        public async Task<Response<List<Cart>>> GetByUserIdAsync( string userId)
        {
            var cartList = await _context.Carts.Where(x => x.UserId == userId).Select(cart => new Cart()
            {
                Id = cart.Id,
                UserId = cart.UserId,
                ProductId = cart.ProductId,
                Quantity = cart.Quantity,
                Product = cart.Product,
            }).ToListAsync();

            if (cartList == null) return new Response<List<Cart>>(false, "No cart found");
            return new Response<List<Cart>>(true, "All cart", cartList);
        }
    }
}
