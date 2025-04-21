
using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.ProductRepositories
{
    public interface IProductRepository
    {
        //Task<Response<List<ProductModel>>> GetAllProductsAsync();
        Task<Response<List<ProductModel>>> SearchProductsAsync(
            string? query,
            string? category,
            int? categoryId,
            string? color,
            string? size,
            string? gender,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1,
            int pageSize = 10);
        Task<Response<ProductModel>> GetProductByIdAsync(int id);
        Task<Response<object>> addProductAsync(ProductModel product);
        Task<Response<object>> UpdateProductAsync(int id, ProductModel product);
        Task<Response<object>> UpdateQuantityAsync(int id, int quantity);
    }
}
