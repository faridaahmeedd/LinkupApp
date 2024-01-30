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
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServices());
			return Ok(services);
		}

		[HttpGet("{ServiceId}")]
		public IActionResult GetService(int ServiceId)
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


		[HttpGet("CustomerRequests/{CustomerId}")]
		public IActionResult GetServicesByCustomer(string CustomerId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound();
			}
			var services = _serviceRepository.GetServicesByCustomer(CustomerId);
			if (services == null)
			{
				return NotFound();
			}
			var mapServices = _mapper.Map<List<ServiceRequestDto>>(services);
			return Ok(mapServices);
		}

		[HttpGet("UncompletedServices")]
		public IActionResult GetUncompletedServices()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var services = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetUncompletedServices());
			return Ok(services);
		}

		[HttpGet("Complete/{ServiceId}")]
        public IActionResult CompleteService(int ServiceId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_serviceRepository.ServiceExist(ServiceId))
            {
                return NotFound(ApiResponse.RequestNotFound);
            }
			if (!_serviceRepository.CompleteService(ServiceId))
			{
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
            return Ok(ApiResponse.ServiceCompletedSuccess);
        }

        [HttpPost("{CustomerId}/{CategoryId}")]
		public IActionResult CreateService(string CustomerId, int CategoryId, [FromBody] ServiceRequestDto serviceRequestDto)
		{
			if (!ModelState.IsValid || serviceRequestDto == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);

			if (!_categoryRepository.CategoryExist(CategoryId))
			{
				return NotFound( ApiResponse.CategoryNotFound);
			}
            if (!_serviceRepository.CheckServiceMinFees(serviceMap, CategoryId))
            {
                return NotFound(ApiResponse.MinFees);  
            }
            serviceMap.Category = _categoryRepository.GetCategory(CategoryId);

			if (!_customerRepository.CustomerExist(CustomerId))
			{
				return NotFound(ApiResponse.UserNotFound);
            }
			serviceMap.Customer = _customerRepository.GetCustomer(CustomerId);
            if (!_customerRepository.CheckCustomerBalance(CustomerId))
            {
                return BadRequest( ApiResponse.PayFine);
            }
            if (!_serviceRepository.CreateService(serviceMap))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(new {
                statusMsg = "success",
                message = "Service Created Successfully.",
				serviceId = serviceMap.Id
            });
		}

		[HttpPut("{ServiceId}")]
		public IActionResult UpdateService(int ServiceId, [FromBody] ServiceRequestDto serviceRequestDto)
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

            if (!_serviceRepository.UpdateService(serviceMap) )
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessUpdated);
		}

		[HttpDelete("{ServiceId}")]
		public IActionResult DeleteService(int ServiceId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound(ApiResponse.RequestNotFound);
			}
			if (!_serviceRepository.DeleteService(ServiceId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}

        [HttpGet("ServiceOffers/{serviceId}")]
        public IActionResult GetOffersOfService(int serviceId)
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
            if (offers != null)
            {
                var offersMap = _mapper.Map<List<ServiceOfferDto>>(offers);
                return Ok(offersMap);
            }
            return NotFound(ApiResponse.OfferNotFound);
        }

        [HttpGet("AcceptedOffer/{serviceId}")]
        public IActionResult GetAcceptedOffer(int serviceId)
        {
			var acceptedOffer = _serviceRepository.AcceptedOffer(serviceId);
            if (acceptedOffer != null)
            {
                var offerMap = _mapper.Map<ServiceOfferDto>(acceptedOffer);
                return Ok(offerMap);
            }
            else
            {
                return NotFound(ApiResponse.OfferNotFound);
            }
        }

		[HttpGet("AllServicesDetails")]
		public IActionResult GetAllServicesDetails()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var serviceDetails = _serviceRepository.GetAllServicesDetails();
			return Ok(serviceDetails);
		}
    }
}
