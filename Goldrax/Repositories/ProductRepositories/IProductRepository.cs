
using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.ProductRepositories
{
    public interface IProductRepository
    {
        Task<Response<List<ProductModel>>> GetAllProductsAsync();
        Task<Response<ProductModel>> GetProductByIdAsync(int id);
        Task<Response<object>> addProductAsync(ProductModel product);
        Task<Response<object>> UpdateProductAsync(int id, ProductModel product);
    }
}
