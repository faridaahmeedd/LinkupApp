using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.APIs;

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
		public IActionResult GetOffers()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var offers = _mapper.Map<List<ServiceOfferDto>>(_offerRepository.GetOffers());
			return Ok(offers);
		}

		[HttpGet("{OfferId}")]
		public IActionResult GetOffer(int OfferId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			var Service = _mapper.Map<ServiceOfferDto>(_offerRepository.GetOffer(OfferId));
			return Ok(Service);
		}

		[HttpPost("{ProviderId}/{RequestId}")]
		public IActionResult CreateOffer(string ProviderId, int RequestId, [FromBody] ServiceOfferDto serviceOfferDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (serviceOfferDto == null)
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

		[HttpPut("{OfferId}")]
		public IActionResult UpdateOffer(int OfferId, [FromBody] ServiceOfferDto serviceOfferDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (serviceOfferDto == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			var serviceMap = _mapper.Map<ServiceOffer>(serviceOfferDto);
			serviceMap.Id = OfferId;

			if (!_offerRepository.UpdateOffer(serviceMap))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessUpdated);
		}

		[HttpPut("Accept/{OfferId}")]
		public IActionResult AcceptOffer(int OfferId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
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

	    [HttpDelete("{OfferId}")]
		public IActionResult DeleteOffer(int OfferId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			if (!_offerRepository.DeleteOffer(OfferId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}


        [HttpGet("providerOffers/{providerId}")]
        public IActionResult GetOffersOfProvider(string providerId)
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_providerRepository.ProviderExist(providerId))
            {
                return NotFound(ApiResponse.UserNotFound);
            }
            var Offers = _mapper.Map < List< ServiceOfferDto >>(_offerRepository.GetOfffersOfProvider(providerId));
            return Ok(Offers);
        }


        [HttpGet("ProviderCanOffer/{providerId}/{requestId}")]
        public IActionResult ProviderCanOffer(string providerId, int requestId)
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_providerRepository.ProviderExist(providerId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			if (!_requestRepository.ServiceExist(requestId))
			{
				return NotFound(ApiResponse.RequestNotFound);
			}
			if (_offerRepository.ProviderAlreadyOffered(providerId, requestId))
			{
				return BadRequest(ApiResponse.AlreadyOffered);
			}
			return Ok(ApiResponse.ProviderCanOffer);
        }
    }
}
