﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Core.Models;
using ServicesApp.Interfaces;
using ServicesApp.Dto.User;
using ServicesApp.Helper;

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
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var customers = _customerRepository.GetCustomers();
				var mapCustomers = _mapper.Map<List<GetCustomerDto>>(customers);
				return Ok(mapCustomers);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}


		[HttpGet("{CustomerId}")]
		public IActionResult GetCustomer(string CustomerId) {
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var customer = _customerRepository.GetCustomer(CustomerId);
				var mapCustomer = _mapper.Map<GetCustomerDto>(customer);
				return Ok(mapCustomer);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Profile/{CustomerId}")]
		public async Task<IActionResult> UpdateProfile(string CustomerId, [FromBody] PostCustomerDto customerUpdate)
		{
			try
			{
				if (!ModelState.IsValid || customerUpdate == null)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var mapCustomer = _mapper.Map<Customer>(customerUpdate);
				mapCustomer.Id = CustomerId;
				await _customerRepository.UpdateCustomer(mapCustomer);
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpDelete("{CustomerId}")]
		public async Task<IActionResult> DeleteCustomer(string CustomerId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				await _customerRepository.DeleteCustomer(CustomerId);
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
	}
}
