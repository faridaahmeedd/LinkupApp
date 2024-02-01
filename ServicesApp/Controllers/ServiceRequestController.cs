using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.APIs;
namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
	[ApiController]
	public class ServiceRequestController : ControllerBase
	{
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public ServiceRequestController(IServiceRequestRepository ServiceRepository,
			ICategoryRepository CategoryRepository, ICustomerRepository customerRepository,
			IMapper mapper)
		{
			_serviceRepository = ServiceRepository;
			_categoryRepository = CategoryRepository;
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
				var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServices());
				return Ok(services);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
        [HttpGet("WithMaxFees")]
        public IActionResult GetServicesWithFees()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServicesWithFees() );
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
				var service = _mapper.Map<ServiceRequestDto>(_serviceRepository.GetService(ServiceId));
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
				var mapServices = _mapper.Map<List<ServiceRequestDto>>(services);
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
				var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetUncompletedServices());
				return Ok(services);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("Complete/{ServiceId}")]
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

        [HttpPost("{CustomerId}/{CategoryId}")]
		public IActionResult CreateService(string CustomerId, int CategoryId, [FromBody] ServiceRequestDto serviceRequestDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceRequestDto == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);

				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponse.CategoryNotFound);
				}
				//if (!_serviceRepository.CheckServiceMinFees(serviceMap, CategoryId))
				//{
				//	return NotFound(ApiResponse.MinFees);
				//}
				serviceMap.Category = _categoryRepository.GetCategory(CategoryId);

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
		public IActionResult UpdateService(int ServiceId, [FromBody] ServiceRequestDto serviceRequestDto)
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

				_serviceRepository.UpdateService(serviceMap);
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

        [HttpGet("ServiceOffers/{serviceId}")]
        public IActionResult GetOffersOfService(int serviceId)
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
				var offers = _serviceRepository.GetOffersOfService(serviceId);
				var offersMap = _mapper.Map<List<ServiceOfferDto>>(offers);
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
				var acceptedOffer = _serviceRepository.AcceptedOffer(serviceId);
				if (acceptedOffer != null)
				{
					var offerMap = _mapper.Map<ServiceOfferDto>(acceptedOffer);
					return Ok(offerMap);
				}
				return NotFound(ApiResponse.OfferNotFound);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("AllServicesDetails")]
		public IActionResult GetAllServicesDetails()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var serviceDetails = _serviceRepository.GetAllServicesDetails();
				return Ok(serviceDetails);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

        [HttpPut("UpdateUnknownCategory/{ServiceId}/{CategoryName}")]
        public IActionResult UpdateServiceCategory(int ServiceId, string CategoryName)
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
                if (!_categoryRepository.CategoryExist(CategoryName))
                {
                    return NotFound(ApiResponse.CategoryNotFound);
                }
                _serviceRepository.UpdateUnkownCategory(ServiceId, CategoryName);
                return Ok(ApiResponse.SuccessUpdated);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpPut("UpdateMaxFees/{ServiceId}/{MaxFees}")]
        public IActionResult UpdateServiceMaxFees(int ServiceId, int MaxFees)
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
                _serviceRepository.UpdateMaxFees(ServiceId, MaxFees);
                return Ok(ApiResponse.SuccessUpdated);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
    }
}
