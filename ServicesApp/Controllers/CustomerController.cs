using Microsoft.AspNetCore.Mvc;
using ServicesApp.Core.Models;
using ServicesApp.Interfaces;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CustomerController : Controller
	{
		private readonly ICustomerRepository _customerRepository;

		public CustomerController(ICustomerRepository customerRepository)
        {
			_customerRepository = customerRepository;
		}

        [HttpGet]
		[ProducesResponseType(200, Type = typeof(Customer))]
		public IActionResult GetCustomers()
		{
			var customers = _customerRepository.GetCustomers();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(customers);
		}

		[HttpGet("{CustomerId}")]
		[ProducesResponseType(200, Type = typeof(Customer))]
		public IActionResult GetCustomer(int CustomerId) {
			if(!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound();
			}
			var customer = _customerRepository.GetCustomer(CustomerId);
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(customer);
		}

		[HttpGet("services/{CustomerId}")]
		[ProducesResponseType(200, Type = typeof(Customer))]
		public IActionResult GetServicesByCustomer(int CustomerId)
		{
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound();
			}
			var services = _customerRepository.GetServicesByCustomer(CustomerId);
			if(services == null)
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(services);
		}

	}
}
