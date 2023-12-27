using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
//TODO : timeslot get   - timeslot update (was 2 slots -- > 1 slot) ? - Image
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
		public IActionResult GetServices()
		{
			var Service = _mapper.Map<List<ServiceRequestDto>>(_serviceRepository.GetServices());
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Service);
		}

		[HttpGet("{ServiceId}")]
		[ProducesResponseType(200, Type = typeof(ServiceRequestDto))]
		public IActionResult GetService(int ServiceId)
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound();
			}
			var Service = _mapper.Map<ServiceRequestDto>(_serviceRepository.GetService(ServiceId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Service);
		}

        [HttpGet("Complete")]
        [ProducesResponseType(200, Type = typeof(ServiceRequestDto))]
        public IActionResult CompleteService(int ServiceId)
        {
            if (!_serviceRepository.ServiceExist(ServiceId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
			if (!_serviceRepository.CompleteService(ServiceId))
			{
                ModelState.AddModelError("", "Something went wrong.");
                return StatusCode(500, ModelState);
            }
            return Ok("Succefully completed");
        }

        //[HttpGet("timeslots/{ServiceId}")]
        //[ProducesResponseType(200, Type = typeof(TimeSlot))]
        //public IActionResult GetTimeSlotsOfRequest(int ServiceId)
        //{
        //	if (!_serviceRepository.ServiceExist(ServiceId))
        //	{
        //		return NotFound();
        //	}

        //	var timeSlots = _mapper.Map<List<TimeSlotDto>>(_serviceRepository.GetTimeSlotsOfRequest(ServiceId));
        //	if (!ModelState.IsValid)
        //	{
        //		return BadRequest(ModelState);
        //	}
        //	return Ok(timeSlots);
        //}

        [HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateService([FromQuery] string CustomerId, [FromQuery] int CategoryId, 
			[FromBody] ServiceRequestDto serviceRequestDto)
		{
			if (serviceRequestDto == null)
			{
				return BadRequest(ModelState);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);

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
			return Created($"/api/ServiceRequest/{serviceMap.Id}", "Service Requested Successfully");
		}

		[HttpPut("update")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult UpdateService([FromQuery] int ServiceId, [FromBody] ServiceRequestDto serviceRequestDto )
		{
			if (serviceRequestDto == null)
			{
				return BadRequest(ModelState);
			}
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);
            serviceMap.Id = ServiceId;

            if (!_serviceRepository.UpdateService(serviceMap) )
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully updated");
		}

		[HttpDelete("{ServiceId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult DeleteService(int ServiceId)
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!_serviceRepository.DeleteService(ServiceId))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully deleted");
		}

        [HttpGet("GetOffersOfService/{id}")]
        [ProducesResponseType(200, Type = typeof(ICollection<ServiceOfferDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetOffersOfService(int id)
        {
           
            if (!_serviceRepository.ServiceExist(id))
            {
                return NotFound();
            }
            var offers = _serviceRepository.GetOffersOfService(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (offers != null)
            {
                var offersMap = _mapper.Map<List<ServiceOfferDto>>(offers);
                return Ok(offersMap);
            }
            return NotFound();
        }

        [HttpGet("accepted-offer")]
        [ProducesResponseType(200, Type = typeof(ServiceOfferDto))]
        [ProducesResponseType(404)]
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
                return NotFound();
            }
        }

    }
}
