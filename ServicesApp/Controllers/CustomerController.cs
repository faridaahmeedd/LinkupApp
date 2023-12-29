using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Core.Models;
using ServicesApp.Dto.Users;
using ServicesApp.Interfaces;
using ServicesApp.APIs;

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
			var mapCustomers = _mapper.Map<List<CustomerDto>>(customers);
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(mapCustomers);
		}


		[HttpGet("{CustomerId}", Name = "GetCustomerById")]
		[ProducesResponseType(200, Type = typeof(Customer))]
		public IActionResult GetCustomer(string CustomerId) {
			if(!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var customer = _customerRepository.GetCustomer(CustomerId);
			var mapCustomer = _mapper.Map<CustomerDto>(customer);

			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(mapCustomer);
		}

		[HttpPost("Profile")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> CreateProfile(CustomerDto customerUpdate, string CustomerId)
		{
			if (customerUpdate == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var mapCustomer = _mapper.Map<Customer>(customerUpdate);
			mapCustomer.Id = CustomerId;
			var result = await _customerRepository.UpdateCustomer(mapCustomer);
			if (! result.Succeeded)
			{
				return StatusCode(500,ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessUpdated);
		}


		[HttpDelete("{CustomerId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteCustomer(string CustomerId)
		{
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var result = await _customerRepository.DeleteCustomer(CustomerId);
			if (!result.Succeeded)
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			
			return Ok(ApiResponse.SuccessDeleted);
		}

	

		//[HttpGet("services/{CustomerId}")]
		//[ProducesResponseType(200, Type = typeof(List<ServiceRequestDto>))]
		//public IActionResult GetServicesByCustomer(string CustomerId)
		//{
		//	if (!_customerRepository.CustomerExist(CustomerId))
		//	{
		//		return NotFound();
		//	}
		//	var services = _customerRepository.GetServicesByCustomer(CustomerId);
		//	if(services == null)
		//	{
		//		return NotFound();
		//	}

		//	var mapServices = _mapper.Map<List<ServiceRequestDto>>(services);
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}
		//	return Ok(mapServices);
		//}
	}
}
