using Goldrax.Repositories.CategoryRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Goldrax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetCategories()
        {
            var categoryList = await _categoryRepository.GetCategoriesAsync();
            if(!categoryList.Succeeded) { return NotFound(categoryList); }
            return Ok(categoryList);
        }
    }
}
