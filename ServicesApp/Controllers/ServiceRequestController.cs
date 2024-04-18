using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.APIs;
using ServicesApp.Core.Models;
namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
	[ApiController]
	public class ServiceRequestController : ControllerBase
	{
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly ISubcategoryRepository _subcategoryRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public ServiceRequestController(IServiceRequestRepository serviceRepository,
			ISubcategoryRepository subcategoryRepository, ICustomerRepository customerRepository,
			IMapper mapper)
		{
			_serviceRepository = serviceRepository;
			_subcategoryRepository = subcategoryRepository;
			_customerRepository = customerRepository;
            _mapper = mapper;
		}

		[HttpGet]
		public IActionResult GetServices()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var services = _mapper.Map<List<GetServiceRequestDto>>(_serviceRepository.GetServices());
				return Ok(services);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{ServiceId}")]
		public IActionResult GetService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				var service = _mapper.Map<GetServiceRequestDto>(_serviceRepository.GetService(ServiceId));
				return Ok(service);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("CustomerRequests/{CustomerId}")]
		public IActionResult GetServicesByCustomer(string CustomerId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				var services = _serviceRepository.GetServicesByCustomer(CustomerId);
				var mapServices = _mapper.Map<List<GetServiceRequestDto>>(services);
				return Ok(mapServices);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("UncompletedServices")]
		public IActionResult GetUncompletedServices()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var services = _mapper.Map<List<GetServiceRequestDto>>(_serviceRepository.GetUncompletedServices());
				return Ok(services);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut("Complete/{ServiceId}")]
        public IActionResult CompleteService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				_serviceRepository.CompleteService(ServiceId);
				return Ok(ApiResponse.ServiceCompletedSuccess);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

        [HttpPost("{CustomerId}/{SubcategoryId}")]
		public IActionResult CreateService(string CustomerId, int SubcategoryId, [FromBody] PostServiceRequestDto serviceRequestDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceRequestDto == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);

				if (!_subcategoryRepository.SubcategoryExist(SubcategoryId))
				{
					return NotFound(ApiResponse.SubcategoryNotFound);
				}
				serviceMap.Subcategory = _subcategoryRepository.GetSubcategory(SubcategoryId);

				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				serviceMap.Customer = _customerRepository.GetCustomer(CustomerId);
				if (!_customerRepository.CheckCustomerBalance(CustomerId))
				{
					return BadRequest(ApiResponse.PayFine);
				}
				_serviceRepository.CreateService(serviceMap);
				return Ok(new
				{
					statusMsg = "success",
					message = "Service Created Successfully.",
					serviceId = serviceMap.Id
				});
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut("{ServiceId}")]
		public IActionResult UpdateService(int ServiceId, [FromBody] PostServiceRequestDto serviceRequestDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceRequestDto == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);
				serviceMap.Id = ServiceId;

				if (!_serviceRepository.UpdateService(serviceMap))
				{
					return BadRequest(ApiResponse.FailedToUpdate);
				}
				return Ok(ApiResponse.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}


		[HttpDelete("{ServiceId}")]
		public IActionResult DeleteService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				_serviceRepository.DeleteService(ServiceId);
				return Ok(ApiResponse.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

        [HttpGet("ServiceUndeclinedOffers/{serviceId}")]
        public IActionResult GetUndeclinedOffersOfService(int serviceId)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_serviceRepository.ServiceExist(serviceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				var offers = _serviceRepository.GetUndeclinedOffersOfService(serviceId);
				var offersMap = _mapper.Map<List<GetServiceOfferDto>>(offers);
				return Ok(offersMap);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

        [HttpGet("AcceptedOffer/{serviceId}")]
        public IActionResult GetAcceptedOffer(int serviceId)
        {
			try
			{
				var acceptedOffer = _serviceRepository.GetAcceptedOffer(serviceId);
				if (acceptedOffer != null)
				{
					var offerMap = _mapper.Map<GetServiceOfferDto>(acceptedOffer);
					return Ok(offerMap);
				}
				return NotFound(ApiResponse.OfferNotFound);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

        [HttpPut("UpdateUnknownSubcategory/{ServiceId}/{SubcategoryName}")]
        public IActionResult UpdateServiceSubcategory(int ServiceId, string SubcategoryName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
					return BadRequest(ApiResponse.NotValid);
                }
                if (!_serviceRepository.ServiceExist(ServiceId))
                {
                    return NotFound(ApiResponse.RequestNotFound);
                }
                if (!_subcategoryRepository.SubcategoryExist(SubcategoryName))
                {
                    return NotFound(ApiResponse.SubcategoryNotFound);
                }
				if (!_serviceRepository.UpdateUnknownSubcategory(ServiceId, SubcategoryName))
				{
					return BadRequest(ApiResponse.NotAuthorized);
				}
                return Ok(ApiResponse.SuccessUpdated);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
		

		[HttpGet("CustomerCalendar/{CustomerId}")]
		public IActionResult GetCalendarByCustomer(string CustomerId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				var calendarDtos = _serviceRepository.GetCalendarDetails(CustomerId);
				return Ok(calendarDtos);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		//[HttpGet("WithMaxFees")]
		//public IActionResult GetServicesWithFees()
		//{
		//    try
		//    {
		//        if (!ModelState.IsValid)
		//        {
		//            return BadRequest(ApiResponse.NotValid);
		//        }
		//        var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServicesWithFees() );
		//        return Ok(services);
		//    }
		//    catch
		//    {
		//        return StatusCode(500, ApiResponse.SomethingWrong);
		//    }
		//}

		//[HttpGet("WithMaxFees/{CustomerId}")]
		//public IActionResult GetServicesWithFees(string CustomerId)
		//{
		//	try
		//	{
		//		if (!ModelState.IsValid)
		//		{
		//			return BadRequest(ApiResponse.NotValid);
		//		}
		//		var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServicesWithFees(CustomerId));
		//		return Ok(services);
		//	}
		//	catch
		//	{
		//		return StatusCode(500, ApiResponse.SomethingWrong);
		//	}
		//}

		//[HttpPut("UpdateMaxFees/{ServiceId}/{MaxFees}")]
		//public IActionResult UpdateServiceMaxFees(int ServiceId, int MaxFees)
		//{
		//	try
		//	{
		//		if (!ModelState.IsValid)
		//		{
		//			return BadRequest(ApiResponse.NotValid);
		//		}
		//		if (!_serviceRepository.ServiceExist(ServiceId))
		//		{
		//			return NotFound(ApiResponse.RequestNotFound);
		//		}
		//		_serviceRepository.UpdateMaxFees(ServiceId, MaxFees);
		//		return Ok(ApiResponse.SuccessUpdated);
		//	}
		//	catch
		//	{
		//		return StatusCode(500, ApiResponse.SomethingWrong);
		//	}
		//}
	}
}
