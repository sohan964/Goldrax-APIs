using Goldrax.Models;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.OrderRepositories
{
    public interface IOrderRepository
    {
        Task<Response<object>> PlaceOrderAsync(OrderModel order);
    }
}
