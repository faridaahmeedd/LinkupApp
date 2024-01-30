using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.APIs;
using Microsoft.AspNetCore.Authorization;

namespace ServicesApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceOfferController : ControllerBase
	{
		private readonly IServiceOfferRepository _offerRepository;
		private readonly IServiceRequestRepository _requestRepository;
		private readonly IProviderRepository _providerRepository;
		private readonly ITimeSlotsRepository _timeSlotsRepository;
		private readonly IMapper _mapper;

		public ServiceOfferController(IServiceRequestRepository RequestRepository, 
			IServiceOfferRepository OfferRepository,
			IProviderRepository ProviderRepository,
			ITimeSlotsRepository TimeSlotsRepository,
			IMapper mapper)
		{
			_offerRepository = OfferRepository;
			_requestRepository = RequestRepository;
			_providerRepository = ProviderRepository;
			_timeSlotsRepository = TimeSlotsRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ServiceOfferDto>))]
		[Authorize(Roles = "Admin, MainAdmin")]
		public IActionResult GetOffers()
		{
			var offers = _mapper.Map<List<ServiceOfferDto>>(_offerRepository.GetOffers());
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(offers);
		}

		[Authorize]
		[HttpGet("{ServiceId}")]
		[ProducesResponseType(200, Type = typeof(ServiceOfferDto))]
		public IActionResult GetOffer(int OfferId)
		{
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			var Service = _mapper.Map<ServiceOfferDto>(_offerRepository.GetOffer(OfferId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(Service);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[Authorize(Roles = "Provider")]
		public IActionResult CreateOffer([FromBody] ServiceOfferDto serviceOfferDto,
			[FromQuery] string ProviderId , [FromQuery]int  RequestId)
		{
			if (serviceOfferDto == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound( ApiResponse.UserNotFound);
			}
			if (!_requestRepository.ServiceExist(RequestId))
			{
				return NotFound( ApiResponse.RequestNotFound);
			}

			if (!_requestRepository.TimeSlotsExistInService(RequestId, serviceOfferDto.TimeSlotId))
			{
				return NotFound(ApiResponse.TimeSlotNotFound);

			}
			var offerMap = _mapper.Map<ServiceOffer>(serviceOfferDto);

			offerMap.Provider = _providerRepository.GetProvider(ProviderId);
			offerMap.Request = _requestRepository.GetService(RequestId);

			if (_timeSlotsRepository.GetTimeSlot(serviceOfferDto.TimeSlotId) == null)
			{
                return NotFound(ApiResponse.TimeSlotNotFound);
            }
            //if ( _offerRepository.ProviderAlreadyOffered(ProviderId, RequestId)) //->
            //{
            //    return BadRequest (ApiResponse.
			//    );
            //}
            if (!_timeSlotsRepository.CheckConflict(offerMap))
			{
				return StatusCode(500, ApiResponse.TimeSlotConflict);
			}
            if (!_providerRepository.CheckProviderBalance(ProviderId))  //->
            {
                return BadRequest(ApiResponse.PayFine);
            }

            if (!_offerRepository.CreateOffer(offerMap))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(new
			{
                statusMsg = "success",
                message = "Offer Created Successfully.",
				OfferId = offerMap.Id
            });
		}

		[HttpPut()]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Roles = "Provider")]
		public IActionResult UpdateOffer([FromQuery] int OfferId, [FromBody] ServiceOfferDto serviceOfferDto)
		{
			if (serviceOfferDto == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var serviceMap = _mapper.Map<ServiceOffer>(serviceOfferDto);
			serviceMap.Id = OfferId;

			if (!_offerRepository.UpdateOffer(serviceMap))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessUpdated);
		}

		[HttpPut("Accept")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[Authorize(Roles = "Customer")]
		public IActionResult AcceptOffer([FromQuery] int OfferId)
		{
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_offerRepository.AcceptOffer(OfferId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			if (!_timeSlotsRepository.UpdateToTime(OfferId))
			{
				return StatusCode(500, ApiResponse.FailedUpdated);
			}
			return Ok(ApiResponse.OfferAccepted);
		}

	    [HttpDelete()]
	    [ProducesResponseType(204)]
	    [ProducesResponseType(400)]
	    [ProducesResponseType(404)]
		[Authorize(Roles = "Provider")]
		public IActionResult DeleteOffer(int OfferId)
		{
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}

			if (!_offerRepository.DeleteOffer(OfferId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}


        [HttpGet("providerOffers/{providerId}")]
        [ProducesResponseType(200, Type = typeof(ServiceOfferDto))]
		[Authorize(Roles = "Provider")]
		public IActionResult GetOffersOfProvider(string providerId)
        {
            if (!_providerRepository.ProviderExist(providerId))
            {
                return NotFound(ApiResponse.UserNotFound);
            }
            var Offers = _mapper.Map < List< ServiceOfferDto >>(_offerRepository.GetOfffersOfProvider(providerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.NotValid);
            }
            return Ok(Offers);
        }


        [HttpGet("providerAlreadyOffered/{providerId}")]
        [ProducesResponseType(200)]
		[Authorize(Roles = "Provider")]
		public IActionResult ProviderAleadyOffer(string providerId , int requestId)
        {
            if (_offerRepository.ProviderAlreadyOffered(providerId , requestId))
            {
                return NotFound(ApiResponse.AlreadyOffered);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.NotValid);
            }
            return Ok(ApiResponse.ProviderCanOffer);
        }
    }
}
