using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.CategoryRepositories
{
    public interface ICategoryRepository
    {
        Task<Response<List<Category>>> GetCategoriesAsync();
        Task<Response<object>> AddCategoryAsync(CategoryModel category);
        Task<Response<object>> AddSubCategoryAsync(SubcategoryModel subcategory);
    }
}
