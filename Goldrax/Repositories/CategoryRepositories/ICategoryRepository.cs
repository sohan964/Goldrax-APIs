using Goldrax.Data;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.CategoryRepositories
{
    public interface ICategoryRepository
    {
        Task<Response<List<Category>>> GetCategoriesAsync();
    }
}
