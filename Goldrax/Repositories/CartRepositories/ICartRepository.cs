using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.CartRepositories
{
    public interface ICartRepository
    {
        Task<Response<object>> AddToCartAsync(CartModel cart);
        Task<Response<object>> UpdateCartQuantityAsync(int id, int? quantity);
    }
}
