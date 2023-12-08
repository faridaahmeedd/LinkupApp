﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class ServiceController : Controller
	{
		private readonly IServiceRepository _serviceRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public ServiceController(IServiceRepository ServiceRepository, ICategoryRepository CategoryRepository, ICustomerRepository customerRepository, IMapper mapper)
		{
			_serviceRepository = ServiceRepository;
			_categoryRepository = CategoryRepository;
			_customerRepository = customerRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Service>))]
		public IActionResult GetServices()
		{
			var Service = _mapper.Map<List<ServiceDto>>(_serviceRepository.GetServices());
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Service);
		}

		[HttpGet("{ServiceId}")]
		[ProducesResponseType(200, Type = typeof(Service))]
		public IActionResult GetService(int ServiceId)
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound();
			}
			var Service = _mapper.Map<ServiceDto>(_serviceRepository.GetService(ServiceId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Service);
		}

		[HttpGet("{ServiceName}/name")]
		[ProducesResponseType(200, Type = typeof(Service))]
		public IActionResult GetService(String ServiceName)
		{
			var Service = _mapper.Map<ServiceDto>(_serviceRepository.GetService(ServiceName));
			if (Service == null)
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Service);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateService([FromQuery] int CustomerId, [FromQuery] int CategoryId, [FromBody] ServiceDto ServiceCreate)
		{
			if (ServiceCreate == null)
			{
				return BadRequest(ModelState);
			}
			var Service = _serviceRepository.GetServices()
				.Where(c => c.Name.Trim().ToUpper() == ServiceCreate.Name.ToUpper())
				.FirstOrDefault();
			if (Service != null)
			{
				ModelState.AddModelError("", "Service already exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var serviceMap = _mapper.Map<Service>(ServiceCreate);

			if (!_categoryRepository.CategoryExist(CategoryId))
			{
				ModelState.AddModelError("", "Category doesn't exist");
				return StatusCode(422, ModelState);
			}
			serviceMap.Category = _categoryRepository.GetCategory(CategoryId);

			if (!_customerRepository.CustomerExist(CustomerId))
			{
				ModelState.AddModelError("", "Customer doesn't exist");
				return StatusCode(422, ModelState);
			}
			serviceMap.Customer = _customerRepository.GetCustomer(CustomerId);

			if (!_serviceRepository.CreateService(serviceMap))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully created");
		}
	}
}
