using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.APIs;
using Azure.Core;

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
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var offers = _mapper.Map<List<ServiceOfferDto>>(_offerRepository.GetOffers());
				return Ok(offers);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{OfferId}")]
		public IActionResult GetOffer(int OfferId)
		{
			try
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
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPost("{ProviderId}/{RequestId}")]
		public IActionResult CreateOffer(string ProviderId, int RequestId, [FromBody] ServiceOfferDto serviceOfferDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceOfferDto == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				if (!_requestRepository.ServiceExist(RequestId))
				{
					return NotFound(ApiResponse.RequestNotFound);
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
				if (_offerRepository.ProviderAlreadyOffered(ProviderId, RequestId)) 
				{
					return BadRequest(ApiResponse.AlreadyOffered);
				}
				if (!_timeSlotsRepository.CheckConflict(offerMap))
				{
					return StatusCode(500, ApiResponse.TimeSlotConflict);
				}
				if (!_providerRepository.CheckProviderBalance(ProviderId))  
				{
					return BadRequest(ApiResponse.PayFine);
				}
                if (!_offerRepository.CheckMaxFees(offerMap))  
                {
                    return BadRequest(ApiResponse.FeesExceededMax);
                }

                _offerRepository.CreateOffer(offerMap);
				return Ok(new
				{
					statusMsg = "success",
					message = "Offer Created Successfully.",
					OfferId = offerMap.Id

				});
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut("{OfferId}")]
		public IActionResult UpdateOffer(int OfferId, [FromBody] ServiceOfferDto serviceOfferDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceOfferDto == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponse.OfferNotFound);
				}
				var offer = _offerRepository.GetOffer(OfferId);
				if (!_requestRepository.TimeSlotsExistInService(offer.Request.Id, serviceOfferDto.TimeSlotId))
				{
					return NotFound(ApiResponse.TimeSlotNotFound);
				}
				var offerMap = _mapper.Map<ServiceOffer>(serviceOfferDto);
				offerMap.Id = OfferId;

				if (!_offerRepository.UpdateOffer(offerMap))
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

		[HttpPut("Accept/{OfferId}")]
		public IActionResult AcceptOffer(int OfferId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponse.OfferNotFound);
				}
				if(!_offerRepository.AcceptOffer(OfferId))
				{
					return StatusCode(500, ApiResponse.FailedToUpdate);
				}
				_timeSlotsRepository.UpdateToTime(OfferId);
				return Ok(ApiResponse.OfferAccepted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

	    [HttpDelete("{OfferId}")]
		public IActionResult DeleteOffer(int OfferId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponse.OfferNotFound);
				}
				_offerRepository.DeleteOffer(OfferId);
				return Ok(ApiResponse.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}


        [HttpGet("ProviderOffers/{providerId}")]
        public IActionResult GetOffersOfProvider(string providerId)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_providerRepository.ProviderExist(providerId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				var Offers = _mapper.Map<List<ServiceOfferDto>>(_offerRepository.GetOfffersOfProvider(providerId));
				return Ok(Offers);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
        }


        [HttpGet("ProviderCanOffer/{providerId}/{requestId}")]
        public IActionResult ProviderCanOffer(string providerId, int requestId)
        {
			try
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
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

        [HttpPut("Decline/{OfferId}")]
        public IActionResult DeclineOffer(int OfferId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_offerRepository.OfferExist(OfferId))
                {
                    return NotFound(ApiResponse.OfferNotFound);
                }
                if (!_offerRepository.DeclineOffer(OfferId))
                {
                    return StatusCode(500, ApiResponse.FailedToUpdate);
                }
                return Ok(ApiResponse.OfferDeclined);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
    }
}
