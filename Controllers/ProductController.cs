using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerCoreApi.Data;
using CustomerCoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CustomerDbContext _dbContext;

        public ProductController(CustomerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _dbContext.Products
                .Include(p => p.ProductDetails)
                .ThenInclude(pd => pd.Customer)
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _dbContext.Products
                .Include(p => p.ProductDetails)
                .ThenInclude(pd => pd.Customer)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (product == null)
            {
                return BadRequest(new { message = "Product data is required" });
            }

            var newProduct = new Product()
            {
                PName = product.PName,
                Description = product.Description
            };

            await _dbContext.Products.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();

            if (product.ProductDetails != null && product.ProductDetails.Any())
            {
                foreach (var pd in product.ProductDetails)
                {
                    var existingCustomer = await _dbContext.Customers.FindAsync(pd.CustomerId);

                    if (existingCustomer != null)
                    {
                        var newProductDetails = new ProductDetails()
                        {
                            CustomerId = pd.CustomerId,
                            ProductId = newProduct.ProductId
                        };

                        await _dbContext.ProductDetails.AddAsync(newProductDetails);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

           
            var createdProduct = await _dbContext.Products
                .Include(p => p.ProductDetails)
                .ThenInclude(pd => pd.Customer)
                .FirstOrDefaultAsync(p => p.ProductId == newProduct.ProductId);

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductId }, createdProduct);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _dbContext.Products
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            existingProduct.PName = product.PName;
            existingProduct.Description = product.Description;

            
            if (product.ProductDetails != null && product.ProductDetails.Any())
            {
               
                _dbContext.ProductDetails.RemoveRange(existingProduct.ProductDetails);

                
                foreach (var pd in product.ProductDetails)
                {
                    var existingCustomer = await _dbContext.Customers.FindAsync(pd.CustomerId);

                    if (existingCustomer != null)
                    {
                        var newProductDetails = new ProductDetails()
                        {
                            CustomerId = pd.CustomerId,
                            ProductId = existingProduct.ProductId
                        };

                        await _dbContext.ProductDetails.AddAsync(newProductDetails);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

          
            var updatedProduct = await _dbContext.Products
                .Include(p => p.ProductDetails)
                .ThenInclude(pd => pd.Customer)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            return Ok(updatedProduct);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var existingProduct = await _dbContext.Products
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            
            if (existingProduct.ProductDetails != null && existingProduct.ProductDetails.Any())
            {
                _dbContext.ProductDetails.RemoveRange(existingProduct.ProductDetails);
            }

           
            _dbContext.Products.Remove(existingProduct);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully", product = existingProduct });
        }
    }
}