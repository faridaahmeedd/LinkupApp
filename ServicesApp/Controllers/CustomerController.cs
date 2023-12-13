using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Core.Models;
using ServicesApp.Dto;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CustomerController : ControllerBase
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

		[HttpGet("{CustomerId}", Name = "GetCustomerById")]
		[ProducesResponseType(200, Type = typeof(Customer))]
		public IActionResult GetCustomer(string CustomerId) {
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

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateProfile([FromBody] CustomerDto CustomerCreate)
		{
			if (CustomerCreate == null)
			{
				return BadRequest(ModelState);
			}
			var customer = _customerRepository.GetCustomers().Where(c => c.Id == CustomerCreate.Id).FirstOrDefault();
			if (customer != null)
			{
				ModelState.AddModelError("", "Customer already exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var customerMap = _mapper.Map<Customer>(CustomerCreate);

			if (!_customerRepository.CreateCustomer(customerMap))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			var url = Url.Link("GetCustomerById", new { CustomerId = CustomerCreate.Id });
			return Created(url, CustomerCreate);
		}


		[HttpGet("services/{CustomerId}")]
		[ProducesResponseType(200, Type = typeof(ServiceRequestDto))]
		public IActionResult GetServicesByCustomer(string CustomerId)
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

			var mapServices = _mapper.Map<List<ServiceRequestDto>>(services);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(mapServices);
		}

		//[HttpPut("update")]
		//[ProducesResponseType(204)]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		//public IActionResult UpdateCustomer([FromBody] Customer customerUpdate)
		//{
		//	if (customerUpdate == null)
		//	{
		//		return BadRequest(ModelState);
		//	}
		//	if (!_customerRepository.CustomerExist(customerUpdate.Id))
		//	{
		//		return NotFound();
		//	}
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	if (!_customerRepository.UpdateCustomer(customerUpdate))
		//	{
		//		ModelState.AddModelError("", "Something went wrong.");
		//		return StatusCode(500, ModelState);
		//	}
		//	return Ok("Successfully updated");
		//}


		//[HttpDelete("{CustomerId}")]
		//[ProducesResponseType(204)]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		//public IActionResult DeleteCustomer(string CustomerId)
		//{
		//	if (!_customerRepository.CustomerExist(CustomerId))
		//	{
		//		return NotFound();
		//	}
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	if (!_customerRepository.DeleteCustomer(CustomerId))
		//	{
		//		ModelState.AddModelError("", "Something went wrong.");
		//		return StatusCode(500, ModelState);
		//	}
		//	return Ok("Successfully deleted");
		//	// TODO : GET SERVICES BY CUSTOMER MAKE SURE THERE IS NO SERVICES BEFORE DELETING CUSTOMER
		//}

	}
}
