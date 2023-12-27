using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using System;

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
		public IActionResult GetOffers()
		{
			var offers = _mapper.Map<List<ServiceOfferDto>>(_offerRepository.GetOffers());
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(offers);
		}

		[HttpGet("{ServiceId}")]
		[ProducesResponseType(200, Type = typeof(ServiceOfferDto))]
		public IActionResult GetOffer(int OfferId)
		{
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound();
			}
			var Service = _mapper.Map<ServiceOfferDto>(_offerRepository.GetOffer(OfferId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Service);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateOffer([FromBody] ServiceOfferDto serviceOfferDto)
		{
			if (serviceOfferDto == null)
			{
				return BadRequest(ModelState);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (!_providerRepository.ProviderExist(serviceOfferDto.ProviderId))
			{
				ModelState.AddModelError("", "Provider doesn't exist");
				return StatusCode(422, ModelState);
			}
			if (!_requestRepository.ServiceExist(serviceOfferDto.RequestId))
			{
				ModelState.AddModelError("", "Service request doesn't exist");
				return StatusCode(422, ModelState);
			}

			if (!_requestRepository.TimeSlotsExistInService(serviceOfferDto.RequestId, serviceOfferDto.TimeSlotId))
			{
				ModelState.AddModelError("", "Time slot is not available for this request");
				return StatusCode(422, ModelState);
			}
			var offerMap = _mapper.Map<ServiceOffer>(serviceOfferDto);

			offerMap.Provider = _providerRepository.GetProvider(serviceOfferDto.ProviderId);
			offerMap.Request = _requestRepository.GetService(serviceOfferDto.RequestId);

			if (_timeSlotsRepository.GetTimeSlot(serviceOfferDto.TimeSlotId) == null)
			{
                ModelState.AddModelError("", "Time slot not found");
                return StatusCode(422, ModelState);
            }

			if (!_timeSlotsRepository.CheckConflict(offerMap))
			{
				ModelState.AddModelError("", "Conflict in time slots");
				return StatusCode(500, ModelState);
			}

			if (!_offerRepository.CreateOffer(offerMap))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Created($"/api/Service/{offerMap.Id}", offerMap);
		}

		[HttpPut()]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult UpdateOffer([FromQuery] int OfferId, [FromBody] ServiceOfferDto serviceOfferDto)
		{
			if (serviceOfferDto == null)
			{
				return BadRequest(ModelState);
			}
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var serviceMap = _mapper.Map<ServiceOffer>(serviceOfferDto);
			serviceMap.Id = OfferId;

			if (!_offerRepository.UpdateOffer(serviceMap))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully updated");
		}

		[HttpPut("Accept")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult AcceptOffer([FromQuery] int OfferId)
		{
		
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (!_offerRepository.AcceptOffer(OfferId))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			if (!_timeSlotsRepository.UpdateToTime(OfferId))
			{
				ModelState.AddModelError("", "Failed to update TimeSlot");
				return StatusCode(500, ModelState);
			}
			return Ok("Offer Accepted");
		}

	   [HttpDelete()]
	   [ProducesResponseType(204)]
	   [ProducesResponseType(400)]
	   [ProducesResponseType(404)]
		public IActionResult DeleteOffer(int OfferId)
		{
			if (!_offerRepository.OfferExist(OfferId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!_offerRepository.DeleteOffer(OfferId))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully deleted");
		}
	}
}
