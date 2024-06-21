using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Repositories;
using ServicesApp.Helper;

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
        private readonly IReviewRepository _reviewRepository;

        public ServiceOfferController(IServiceRequestRepository RequestRepository, 
			IServiceOfferRepository OfferRepository,
			IProviderRepository ProviderRepository,
			ITimeSlotsRepository TimeSlotsRepository,
			IReviewRepository reviewRepository,
			IMapper mapper)
		{
			_offerRepository = OfferRepository;
			_requestRepository = RequestRepository;
			_providerRepository = ProviderRepository;
			_timeSlotsRepository = TimeSlotsRepository;
			_reviewRepository = reviewRepository;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetOffers()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var offers = _mapper.Map<List<GetServiceOfferDto>>(_offerRepository.GetOffers());
                foreach (var offer in offers)
                {
                    offer.ProviderAvgRating = await _reviewRepository.CalculateAvgRating(offer.ProviderId);
                }
                return Ok(offers);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{OfferId}")]
		public async Task<IActionResult> GetOffer(int OfferId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponses.OfferNotFound);
				}
				var offer = _mapper.Map<GetServiceOfferDto>(_offerRepository.GetOffer(OfferId));
             
                offer.ProviderAvgRating = await _reviewRepository.CalculateAvgRating(offer.ProviderId);
                
                return Ok(offer);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPost("{ProviderId}/{RequestId}")]
		public IActionResult CreateOffer(string ProviderId, int RequestId, [FromBody] PostServiceOfferDto serviceOfferDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceOfferDto == null)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				if (!_requestRepository.ServiceExist(RequestId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				if (!_requestRepository.TimeSlotsExistInService(RequestId, serviceOfferDto.TimeSlotId))
				{
					return NotFound(ApiResponses.TimeSlotNotFound);
				}
				if (_offerRepository.ProviderAlreadyOffered(ProviderId, RequestId))
				{
					return BadRequest(ApiResponses.AlreadyOffered);
				}

				var offerMap = _mapper.Map<ServiceOffer>(serviceOfferDto);
				offerMap.Provider = _providerRepository.GetProvider(ProviderId);
				offerMap.Request = _requestRepository.GetService(RequestId);

				if (!_timeSlotsRepository.CheckConflict(offerMap))
				{
					return StatusCode(500, ApiResponses.TimeSlotConflict);
				}
				if (!_offerRepository.CheckFeesRange(offerMap))
				{
					return BadRequest(ApiResponses.FeesOutsideRange);
				}
				//if (!_providerRepository.CheckProviderBalance(ProviderId))  
				//{
				//	return BadRequest(ApiResponse.PayBalance);
				//}

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
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("{OfferId}")]
		public IActionResult UpdateOffer(int OfferId, [FromBody] PostServiceOfferDto serviceOfferDto)
		{
			try
			{
				if (!ModelState.IsValid || serviceOfferDto == null)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponses.OfferNotFound);
				}
				var offer = _offerRepository.GetOffer(OfferId);
				if (!_requestRepository.TimeSlotsExistInService(offer.Request.Id, serviceOfferDto.TimeSlotId))
				{
					return NotFound(ApiResponses.TimeSlotNotFound);
				}

				var offerMap = _mapper.Map<ServiceOffer>(serviceOfferDto);
				offerMap.Id = OfferId;
				offerMap.Request = _offerRepository.GetOffer(OfferId).Request;
				if (!_offerRepository.CheckFeesRange(offerMap))
				{
					return BadRequest(ApiResponses.FeesOutsideRange);
				}
				if (_offerRepository.UpdateOffer(offerMap))
				{
					return Ok(ApiResponses.SuccessUpdated);
				}
				return BadRequest(ApiResponses.FailedToUpdate);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Accept/{OfferId}")]
		public IActionResult AcceptOffer(int OfferId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponses.OfferNotFound);
				}
				if(!_offerRepository.AcceptOffer(OfferId))
				{
					return StatusCode(500, ApiResponses.FailedToUpdate);
				}
				_timeSlotsRepository.UpdateToTime(OfferId);
				return Ok(ApiResponses.OfferAccepted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Decline/{OfferId}")]
		public IActionResult DeclineOffer(int OfferId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponses.OfferNotFound);
				}
				if (!_offerRepository.DeclineOffer(OfferId))
				{
					return StatusCode(500, ApiResponses.FailedToUpdate);
				}
				return Ok(ApiResponses.OfferDeclined);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpDelete("{OfferId}")]
		public IActionResult DeleteOffer(int OfferId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_offerRepository.OfferExist(OfferId))
				{
					return NotFound(ApiResponses.OfferNotFound);
				}
				if (!_offerRepository.DeleteOffer(OfferId))
				{
					return NotFound(ApiResponses.FailedToDelete);
				}
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}


        [HttpGet("ProviderOffers/{ProviderId}")]
        public async Task<IActionResult> GetOffersOfProvider(string ProviderId)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var Offers = _mapper.Map<List<GetServiceOfferDto>>(_offerRepository.GetOfffersOfProvider(ProviderId));

                foreach (var offer in Offers)
                {
                    offer.ProviderAvgRating = await _reviewRepository.CalculateAvgRating(offer.ProviderId);
                }

                return Ok(Offers);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
        }


        [HttpGet("ProviderCanOffer/{ProviderId}/{RequestId}")]
        public IActionResult ProviderCanOffer(string ProviderId, int RequestId)
        {
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				if (!_requestRepository.ServiceExist(RequestId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				if (_offerRepository.ProviderAlreadyOffered(ProviderId, RequestId))
				{
					return BadRequest(ApiResponses.AlreadyOffered);
				}
				return Ok(ApiResponses.ProviderCanOffer);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("ProviderCalendar/{ProviderId}")]
		public IActionResult GetCalendarByProvider(string ProviderId)
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
				var calendarDtos = _offerRepository.GetCalendarDetails(ProviderId);
				return Ok(calendarDtos);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
    }
}
