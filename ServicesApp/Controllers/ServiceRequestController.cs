using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Helper;
using ServicesApp.Repository;
namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
	[ApiController]
	public class ServiceRequestController : ControllerBase
	{
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly ISubcategoryRepository _subcategoryRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IProviderRepository _providerRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;

		public ServiceRequestController(IServiceRequestRepository serviceRepository,
			ISubcategoryRepository subcategoryRepository, ICustomerRepository customerRepository,
			IProviderRepository providerRepository, IReviewRepository reviewRepository , IMapper mapper)
		{
			_serviceRepository = serviceRepository;
			_subcategoryRepository = subcategoryRepository;
			_customerRepository = customerRepository;
			_providerRepository = providerRepository;
			_reviewRepository = reviewRepository;
			_mapper = mapper;
		}

		[HttpGet]
		public IActionResult GetServices()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var services = _mapper.Map<List<GetServiceRequestDto>>(_serviceRepository.GetServices());
				return Ok(services);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{ServiceId}")]
		public IActionResult GetService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				var service = _mapper.Map<GetServiceRequestDto>(_serviceRepository.GetService(ServiceId));
				return Ok(service);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
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
					return NotFound(ApiResponses.UserNotFound);
				}
				var services = _serviceRepository.GetServicesByCustomer(CustomerId);
				var mapServices = _mapper.Map<List<GetServiceRequestDto>>(services);
				return Ok(mapServices);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Complete/{ServiceId}")]
        public IActionResult CompleteService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				_serviceRepository.CompleteService(ServiceId);
				return Ok(ApiResponses.ServiceCompletedSuccess);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

        [HttpPost("{CustomerId}/{SubcategoryId}")]
		public IActionResult CreateService(string CustomerId, int SubcategoryId, [FromBody] PostServiceRequestDto serviceRequestDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceRequestDto == null)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);

				if (!_subcategoryRepository.SubcategoryExist(SubcategoryId))
				{
					return NotFound(ApiResponses.SubcategoryNotFound);
				}
				serviceMap.Subcategory = _subcategoryRepository.GetSubcategory(SubcategoryId);

				if (!_customerRepository.CustomerExist(CustomerId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				serviceMap.Customer = _customerRepository.GetCustomer(CustomerId);
				//if (!_customerRepository.CheckCustomerBalance(CustomerId))
				//{
				//	return BadRequest(ApiResponse.PayBalance);
				//}
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
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPost("AfterExamination/{ServiceId}")]
		public IActionResult CreateRequestAfterExamination(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				var acceptedOffer = _serviceRepository.GetAcceptedOffer(ServiceId);
				if (acceptedOffer != null && acceptedOffer.Examination == false)
				{
					return BadRequest(ApiResponses.NotExamination);
				}
				int? newServiceId = _serviceRepository.CreateRequestAfterExamination(ServiceId);
				if(newServiceId != null)
				{
					return Ok(new
					{
						statusMsg = "success",
						message = "Service Created Successfully.",
						serviceId = newServiceId
					});
				}
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("{ServiceId}")]
		public IActionResult UpdateService(int ServiceId, [FromBody] PostServiceRequestDto serviceRequestDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceRequestDto == null)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				var serviceMap = _mapper.Map<ServiceRequest>(serviceRequestDto);
				serviceMap.Id = ServiceId;
				
				if (!_serviceRepository.UpdateService(serviceMap))
				{
					return BadRequest(ApiResponses.FailedToUpdate);
				}
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("ExaminationComment/{ServiceId}/{Comment}")]
		public IActionResult AddExaminationComment(int ServiceId, string Comment)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				var acceptedOffer = _serviceRepository.GetAcceptedOffer(ServiceId);
				if (acceptedOffer != null && acceptedOffer.Examination == false)
				{
					return BadRequest(ApiResponses.NotExamination);
				}
				if (!_serviceRepository.AddExaminationComment(ServiceId, Comment))
				{
					return BadRequest(ApiResponses.FailedToUpdate);
				}
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Emergency/{ServiceId}/{EmergencyType}")]
		public IActionResult AddEmergency(int ServiceId, string EmergencyType)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				if (!_serviceRepository.AddEmergency(ServiceId, EmergencyType))
				{
					return BadRequest(ApiResponses.FailedToUpdate);
				}
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}


		[HttpDelete("{ServiceId}")]
		public IActionResult DeleteService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				if(!_serviceRepository.DeleteService(ServiceId))
				{
					return BadRequest(ApiResponses.FailedToDelete);
				}
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

        [HttpGet("ServiceUndeclinedOffers/{ServiceId}")]
        public async Task<IActionResult> GetUndeclinedOffersOfService(int ServiceId)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				var offers = _serviceRepository.GetUndeclinedOffersOfService(ServiceId);
				var offersMap = _mapper.Map<List<GetServiceOfferDto>>(offers);

                foreach (var offer in offersMap)
                {
                    offer.ProviderAvgRating = await _reviewRepository.CalculateAvgRating(offer.ProviderId);
                }
                return Ok(offersMap);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

        [HttpGet("AcceptedOffer/{ServiceId}")]
        public async Task<IActionResult> GetAcceptedOffer(int ServiceId)
        {
			try
			{
				var acceptedOffer = _serviceRepository.GetAcceptedOffer(ServiceId);
				if (acceptedOffer != null)
				{
					var offerMap = _mapper.Map<GetServiceOfferDto>(acceptedOffer);

                    offerMap.ProviderAvgRating = await _reviewRepository.CalculateAvgRating(offerMap.ProviderId);
                    
                    return Ok(offerMap);
				}
				return NotFound(ApiResponses.OfferNotFound);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

        [HttpPut("UpdateUnknownSubcategory/{ServiceId}/{SubcategoryName}")]
        public IActionResult UpdateServiceSubcategory(int ServiceId, string SubcategoryName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
					return BadRequest(ApiResponses.NotValid);
                }
                if (!_serviceRepository.ServiceExist(ServiceId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }
                if (!_subcategoryRepository.SubcategoryExist(SubcategoryName))
                {
                    return NotFound(ApiResponses.SubcategoryNotFound);
                }
				if (!_serviceRepository.UpdateUnknownSubcategory(ServiceId, SubcategoryName))
				{
					return BadRequest(ApiResponses.NotAuthorized);
				}
                return Ok(ApiResponses.SuccessUpdated);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
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
					return NotFound(ApiResponses.UserNotFound);
				}
				var calendarDtos = _serviceRepository.GetCalendarDetails(CustomerId);
				return Ok(calendarDtos);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

        [HttpPost("Images/{ServiceId}")]
        public IActionResult AddImages(int ServiceId, [FromBody] ICollection<ImageDto> imagesDto)
        {
            try
            {
                if (!ModelState.IsValid || imagesDto == null)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_serviceRepository.ServiceExist(ServiceId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }
				if (_serviceRepository.GetImagesOfService(ServiceId).Count + imagesDto.Count > 5)
				{
					return BadRequest(ApiResponses.ImagesExceededMax);
				}
				List<Image> mapImage = new List<Image>();
                foreach (var item in imagesDto)
                {
                    var mapItem = _mapper.Map<Image>(item);
                    mapItem.ServiceRequest = _serviceRepository.GetService(ServiceId);
                    mapImage.Add(mapItem);
                }
                _serviceRepository.AddImages(mapImage);
                return Ok(ApiResponses.SuccessCreated);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("Images/{ServiceId}")]
        public IActionResult GetImageOfService(int ServiceId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_serviceRepository.ServiceExist(ServiceId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }
                var Image = _mapper.Map<List<ImageDto>>(_serviceRepository.GetImagesOfService(ServiceId));
                return Ok(Image);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

		[HttpDelete("Images/{ImageId}")]
		public IActionResult DeleteImage(int ImageId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_serviceRepository.DeleteImage(ImageId))
				{
					return StatusCode(500, ApiResponses.FailedToDelete);
				}
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
            {
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("MatchedRequestsOfProvider/{ProviderId}")]
		public IActionResult GetMatchedRequestsOfProvider(string ProviderId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var services = _serviceRepository.GetMatchedRequestsOfProvider(ProviderId);
				var mapServices = _mapper.Map<List<GetServiceRequestDto>>(services);
				return Ok(mapServices);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("UncompletedServices")]
		public IActionResult GetUncompletedServices()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var services = _mapper.Map<List<GetServiceRequestDto>>(_serviceRepository.GetUncompletedServices());
				return Ok(services);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
	}
}
