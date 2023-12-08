using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Core.Models;
using ServicesApp.Dto;
using ServicesApp.Interfaces;
using ServicesApp.Repository;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CustomerController : Controller
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public CustomerController(ICustomerRepository customerRepository, IMapper mapper)
        {
			_customerRepository = customerRepository;
			_mapper = mapper;
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
		[ProducesResponseType(200, Type = typeof(ServiceDto))]
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

			var mapServices = _mapper.Map<List<ServiceDto>>(services);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(mapServices);
		}

	}
}
