
using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Repositories.ProductRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Goldrax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("allproducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productRepository.GetAllProductsAsync();
            if(!result.Succeeded ) { 
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var result = await _productRepository.GetProductByIdAsync(id);
            if(!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel product)
        {
            var result = await _productRepository.addProductAsync(product);
            if(!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel product)
        {
            var res = await _productRepository.UpdateProductAsync(id, product);
            if(!res.Succeeded) return BadRequest(res);
            return Ok(res);
        }


    }
}
