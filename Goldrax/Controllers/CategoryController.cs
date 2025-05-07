using Goldrax.Data;
using Goldrax.Models;
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

        [HttpPost("addnew")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryModel category)
        {
            var result = await _categoryRepository.AddCategoryAsync(category);
            if(!result.Succeeded) { return BadRequest(result); }
            return Ok(result);
        }

        [HttpPost("addnewSubcategory")]
        public async Task<IActionResult> AddSubCategory([FromBody] SubcategoryModel subcategory)
        {
            var result = await _categoryRepository.AddSubCategoryAsync(subcategory);

            if(!result.Succeeded) { return BadRequest(result); }
            return Ok(result);

        }
    }
}
