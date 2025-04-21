using Goldrax.Data;
using Goldrax.Models;
using Goldrax.Models.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Goldrax.Repositories.ProductRepositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<Response<List<ProductModel>>> GetProductsAsync(
        //    stirng?)
        //{
        //    var allProducts = await _context.Products.Select(x => new ProductModel
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Description = x.Description,
        //        Price = x.Price,
        //        Brand = x.Brand,
        //        Category = new
        //        {
        //            x.CategoryId,
        //            x.Category.Name,
        //        },

        //        Size = x.Size,
        //        Color = x.Color,
        //        Discount = x.Discount,
        //        Subcategory = new
        //        {
        //            x.SubcategoryId,
        //            x.Subcategory.Name,
        //        },

        //        Gender = x.Gender,
        //        Image = x.Image,
        //        Quantity = x.Quantity,
        //        Seller = new
        //        {
        //            x.SellerId,
        //            x.Seller.FullName,
        //            x.Seller.Email,
        //        }
        //    }).ToListAsync();
        //    //return allProducts;
        //    //allProducts.Clear();
        //    if (allProducts.Count == 0)
        //    {
        //        return new Response<List<ProductModel>>(false, "Products Not found");
        //    }
        //    return new Response<List<ProductModel>>(true, "All Products", allProducts);
        //}


        //search products
        public async Task<Response<List<ProductModel>>> SearchProductsAsync(
    string? query,
    string? category,
    int? categoryId,
    string? color,
    string? size,
    string? gender,
    decimal? minPrice,
    decimal? maxPrice,
    int page = 1,
    int pageSize = 10)
        {
            var productsQuery = _context.Products
                .Where(p =>
                    (string.IsNullOrEmpty(query) || p.Name.Contains(query)) &&
                    (string.IsNullOrEmpty(category) || p.Category.Name == category) &&
                    (string.IsNullOrEmpty(category) || p.Category.Id == categoryId) &&
                    (string.IsNullOrEmpty(color) || p.Color == color) &&
                    (string.IsNullOrEmpty(size) || p.Size == size) &&
                    (string.IsNullOrEmpty(gender) || p.Gender == gender) &&
                    (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice.Value)
                );

            var totalCount = await productsQuery.CountAsync(); // total for frontend pagination

            var filteredProducts = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Brand = x.Brand,
                    CategoryId = x.CategoryId,
                    Category = new
                    {
                        x.CategoryId,
                        x.Category.Name,
                    },
                    Size = x.Size,
                    Color = x.Color,
                    Discount = x.Discount,
                    SubcategoryId = (int)x.SubcategoryId,
                    Subcategory = new
                    {
                        x.SubcategoryId,
                        x.Subcategory.Name,
                    },
                    Gender = x.Gender,
                    Image = x.Image,
                    Quantity = x.Quantity,
                    Seller = new
                    {
                        x.SellerId,
                        x.Seller.FullName,
                        x.Seller.Email,
                    }
                })
                .ToListAsync();

            if (filteredProducts.Count == 0)
            {
                return new Response<List<ProductModel>>(false, "No matching products found.");
            }

            return new Response<List<ProductModel>>(true, "Filtered Products", filteredProducts);
        }



        public async Task<Response<ProductModel>> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.Where(x => x.Id ==id).Select(x => new ProductModel() {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Brand = x.Brand,
                Category = new
                {
                    x.CategoryId,
                    x.Category.Name,
                },

                Size = x.Size,
                Color = x.Color,
                Discount = x.Discount,
                Subcategory = new
                {
                    x.SubcategoryId,
                    x.Subcategory.Name,
                },

                Gender = x.Gender,
                Image = x.Image,
                Quantity = x.Quantity,
                Seller = new
                {
                    x.SellerId,
                    x.Seller.FullName,
                    x.Seller.Email,
                }
            }).FirstOrDefaultAsync();
            if (product == null)
            {
                return new Response<ProductModel>(false, "Product Not found");
            }
            return new Response<ProductModel>(true, "Product Not found", product);
        }

        //addnewProduct
        public async Task<Response<object>> addProductAsync(ProductModel product)
        {
            var addNew = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                Size = product.Size,
                Color = product.Color,
                SellerId = product.SellerId,
                SubcategoryId = product.SubcategoryId,
                Gender = product.Gender,
                Image = product.Image,
                Discount = string.IsNullOrEmpty(product.Discount) ? "0" : product.Discount,
                Quantity = product.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _context.AddAsync(addNew);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return new Response<object>(false, "Failed to add new product");
            }

            return new Response<object>(true, "Product added successfully");
        }

        //updateProduct
        public async Task<Response<object>> UpdateProductAsync(int id, ProductModel product)
        {
            var exProduct = await _context.Products.FindAsync(id);
            if (exProduct == null)
            {
                return new Response<object>(false, "Product not found");
            }

            exProduct.Name = product.Name;
            exProduct.Description = product.Description;
            exProduct.Price = product.Price;
            exProduct.Brand = product.Brand;
            exProduct.CategoryId = product.CategoryId;
            exProduct.Size = product.Size;
            exProduct.Color = product.Color;
            exProduct.Discount = product.Discount;
            exProduct.Quantity = product.Quantity;
            exProduct.UpdatedAt = DateTime.UtcNow;
            exProduct.Gender = product.Gender;
            exProduct.Image = product.Image;
            exProduct.SellerId = product.SellerId;
            exProduct.SubcategoryId = product.SubcategoryId;

                

            var res = await _context.SaveChangesAsync();
            if (res == 0)
            {
                return new Response<object>(false, "Failed to Update Product");
            }
            return new Response<object>(true, "Success Fully update the product");

        }

        public async Task<Response<object>> UpdateQuantityAsync (int id, int quantity)
        {
            var findProduct = await _context.Products.FindAsync (id);
            if (findProduct == null)
            {
                return new Response<object>(false, "Product not found");
            }

            findProduct.Quantity = quantity;
            var res = await _context.SaveChangesAsync();
            if(res == 0)
            {
                return new Response<object>(false, "Queantity update failed");
            }
            return new Response<object>(true, $"Now existing queantity is {findProduct.Quantity}");
        }

    }
}
