using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.APIs;
using Microsoft.AspNetCore.Authorization;
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
		[ProducesResponseType(200, Type = typeof(IEnumerable<ServiceRequestDto>))]
		[Authorize(Roles = "Admin,MainAdmin")]
		public IActionResult GetServices()
		{
			var Service = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServices());
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(Service);
		}

		[HttpGet("{ServiceId}")]
		[ProducesResponseType(200, Type = typeof(ServiceRequestDto))]
		[Authorize]
		public IActionResult GetService(int ServiceId)
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound(ApiResponse.RequestNotFound);
			}
			var Service = _mapper.Map<ServiceRequestDto>(_serviceRepository.GetService(ServiceId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(Service);
		}


		[HttpGet("CustomerRequests/{CustomerId}")]
		[ProducesResponseType(200, Type = typeof(List<ServiceRequestDto>))]
		[Authorize(Roles = "Customer,Admin,MainAdmin")]
		public IActionResult GetServicesByCustomer(string CustomerId)
		{
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
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(mapServices);
		}

		[HttpGet("Complete")]
        [ProducesResponseType(200, Type = typeof(ServiceRequestDto))]
		[Authorize(Roles = "Provider")]
		public IActionResult CompleteService(int ServiceId)
        {
            if (!_serviceRepository.ServiceExist(ServiceId))
            {
                return NotFound(ApiResponse.RequestNotFound);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.NotValid);
            }
			if (!_serviceRepository.CompleteService(ServiceId))
			{
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
            return Ok(ApiResponse.ServiceCompletedSuccess);
        }

        [HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[Authorize(Roles = "Customer")]
		public IActionResult CreateService([FromQuery] string CustomerId, [FromQuery] int CategoryId, 
			[FromBody] ServiceRequestDto serviceRequestDto)
		{
			if (serviceRequestDto == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}

			if (!ModelState.IsValid)
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

		[HttpPut("update")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Roles = "Customer")]
		public IActionResult UpdateService([FromQuery] int ServiceId, [FromBody] ServiceRequestDto serviceRequestDto )
		{
			if (serviceRequestDto == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound(ApiResponse.RequestNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
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
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Roles = "Customer,Admin,MainAdmin")]
		public IActionResult DeleteService(int ServiceId)
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound(ApiResponse.RequestNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_serviceRepository.DeleteService(ServiceId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}

        [HttpGet("GetOffersOfService/{id}")]
        [ProducesResponseType(200, Type = typeof(ICollection<ServiceOfferDto>))]
        [ProducesResponseType(404)]
		[Authorize(Roles = "Customer,Admin,MainAdmin")]
		public IActionResult GetOffersOfService(int id)
        {
            if (!_serviceRepository.ServiceExist(id))
            {
                return NotFound(ApiResponse.RequestNotFound);
            }
            var offers = _serviceRepository.GetOffersOfService(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.NotValid);
            }
            if (offers != null)
            {
                var offersMap = _mapper.Map<List<ServiceOfferDto>>(offers);
                return Ok(offersMap);
            }
            return NotFound(ApiResponse.OfferNotFound);
        }

        [HttpGet("accepted-offer")]
        [ProducesResponseType(200, Type = typeof(ServiceOfferDto))]
        [ProducesResponseType(404)]
		[Authorize(Roles = "Customer,Admin,MainAdmin")]
		public IActionResult GetAcceptedOffer(int serviceId)
        {
            var acceptedOffer = _serviceRepository.AcceptedOffer(serviceId);

            if (acceptedOffer != null)
            {
                var offersMap = _mapper.Map<ServiceOfferDto>(acceptedOffer);

                return Ok(offersMap);
            }
            else
            {
                return NotFound(ApiResponse.OfferNotFound);
            }
        }


        [HttpGet("AllServicesDetails")]
		[Authorize(Roles = "Admin,MainAdmin")]
		public IActionResult GetAllServicesDetails()
        {
            var serviceDetails = _serviceRepository.GetAllServicesDetails();

            if (serviceDetails == null || serviceDetails.Count == 0)
            {
                return NotFound(ApiResponse.RequestNotFound);
            }

            return Ok(serviceDetails);
        }

    }
}
