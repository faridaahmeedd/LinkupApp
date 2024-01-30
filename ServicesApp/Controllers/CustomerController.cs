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
		public IActionResult GetCustomers()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var customers = _customerRepository.GetCustomers();
			var mapCustomers = _mapper.Map<List<CustomerDto>>(customers);
			return Ok(mapCustomers);
		}


		[HttpGet("{CustomerId}", Name = "GetCustomerById")]
		public IActionResult GetCustomer(string CustomerId) {
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var customer = _customerRepository.GetCustomer(CustomerId);
			var mapCustomer = _mapper.Map<CustomerDto>(customer);
			return Ok(mapCustomer);
		}

		[HttpPut("Profile/{CustomerId}")]
		public async Task<IActionResult> UpdateProfile(string CustomerId, [FromBody] CustomerDto customerUpdate)
		{
			if (!ModelState.IsValid || customerUpdate == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
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
		public async Task<IActionResult> DeleteCustomer(string CustomerId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var result = await _customerRepository.DeleteCustomer(CustomerId);
			if (!result.Succeeded)
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}
	}
}
