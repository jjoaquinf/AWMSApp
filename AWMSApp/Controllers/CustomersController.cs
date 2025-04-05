using AWMSApp.Repository;
using DBData.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Reflection;

namespace AWMSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await customerRepository.GetCustomersAsync();
        }

        [Authorize]
        [HttpPatch]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> FindCustomers([FromQuery] string searchTerm)
        {
            return await customerRepository.FindCustomer(searchTerm);
        }

        //GET: api/Customers/5
        [Authorize]
        [HttpGet("{id}", Name = "GetCustomer")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string id)
        {
            Customer? customer = await customerRepository.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // POST: api/Customers
        [Authorize]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            Customer? added = await customerRepository.CreateAsync(customer);
            if (added == null)
            {
                return BadRequest();
            }
            return CreatedAtRoute(nameof(GetCustomer), new { id = added.CustomerId }, added);
        }

        // PUT: api/Customers/5
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomer(string id, [FromBody] Customer customer)
        {
            id = id.ToUpper();
            if (customer == null || id != customer.CustomerId)
            {
                return BadRequest();
            }
            if (await customerRepository.UpdateAsync(id, customer) == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/Customers/5
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            id = id.ToUpper();
            Customer? customer = await customerRepository.DeleteAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
