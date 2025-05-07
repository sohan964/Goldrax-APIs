using Goldrax.Data;
using Goldrax.Models;
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
            var categories = await _context.Categories.Include(sub => sub.Subcategories).Select(category => new Category()
            {
                Id = category.Id,
                Name = category.Name,
                Subcategories = category.Subcategories.Select(s => new Subcategory
                {
                    Id=s.Id,
                    Name = s.Name,
                }).ToList()
            }).ToListAsync();

            if(categories == null || categories.Count == 0)
            {
                return new Response<List<Category>>(false, "Categories Not found");
            }
            return new Response<List<Category>>(true, "all Categories", categories);
        }

        public async Task<Response<object>> AddCategoryAsync(CategoryModel category)
        {

            var newCategory = new Category() {
               Name = category.Name,
               Description = category.Description,
               CreatedAt = DateTime.UtcNow,
               UpdatedAt = DateTime.UtcNow
            
            };

            await _context.Categories.AddAsync(newCategory);
            var res = await _context.SaveChangesAsync();
            if(res == 0) { return new Response<object>(false, "Fail to add new"); }
            return new Response<object>(true, "New Category added Success");

        }

        public async Task<Response<object>> AddSubCategoryAsync( SubcategoryModel subcategory)
        {
            var exCategory = await _context.Categories.FindAsync(subcategory.CategoryId);
            if(exCategory == null) { return new Response<object>(false, "Category Not Found"); }
            var newSubcategory = new Subcategory()
            {
                Name = subcategory.Name,
                Description = subcategory.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryId = subcategory.CategoryId
            };

            await _context.Subcategories.AddAsync(newSubcategory);
            var res = await _context.SaveChangesAsync();
            if(res == 0) { return new Response<object>(false, "SubCategory Fail to add"); }
            return new Response<object>(true, "Add New Success");
        }
    }
}
