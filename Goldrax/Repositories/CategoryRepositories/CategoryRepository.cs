using Goldrax.Data;
using Goldrax.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Goldrax.Repositories.CategoryRepositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<Category>>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.Select(category => new Category()
            {
                Id = category.Id,
                Name = category.Name,
            }).ToListAsync();

            if(categories == null || categories.Count == 0)
            {
                return new Response<List<Category>>(false, "Categories Not found");
            }
            return new Response<List<Category>>(true, "all Categories", categories);
        }

    }
}
