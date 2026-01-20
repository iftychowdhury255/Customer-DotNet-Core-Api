using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerCoreApi.Data;
using CustomerCoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _dbContext;

        public CustomerController(CustomerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _dbContext.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCustomer = new Customer()
            {
                Name = customer.Name,
                Email = customer.Email,
                Address = customer.Address,
                TotalOrders = customer.TotalOrders
            };

            await _dbContext.Customers.AddAsync(newCustomer);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomer = await _dbContext.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.Address = customer.Address;
            existingCustomer.TotalOrders = customer.TotalOrders;

            await _dbContext.SaveChangesAsync();

            return Ok(existingCustomer);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var existingCustomer = await _dbContext.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            _dbContext.Customers.Remove(existingCustomer);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Customer deleted successfully", customer = existingCustomer });
        }
    }
}