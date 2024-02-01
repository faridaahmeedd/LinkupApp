using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Models;
using ServicesApp.Interfaces;
using AutoMapper;
using ServicesApp.APIs;
namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotsRepository _timeSlotRepository;
		private readonly IServiceRequestRepository _requestRepository;
		private readonly IMapper _mapper;

        public TimeSlotController(ITimeSlotsRepository timeSlotRepository , IServiceRequestRepository requestRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
			_requestRepository = requestRepository;
            _mapper = mapper;
        }

		[HttpGet("{TimeSlotId}")]
		public IActionResult GetTimeSlot(int TimeSlotId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_timeSlotRepository.TimeSlotExist(TimeSlotId))
				{
					return NotFound(ApiResponse.TimeSlotNotFound);
				}
				var TimeSlot = _mapper.Map<TimeSlotDto>(_timeSlotRepository.GetTimeSlot(TimeSlotId));
				return Ok(TimeSlot);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}


		[HttpGet("service/{ServiceId}")]
		public IActionResult GetTimeSlotsOfService(int ServiceId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_requestRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				var TimeSlot = _mapper.Map<List<TimeSlotDto>>(_timeSlotRepository.GetTimeSlotsOfService(ServiceId));
				return Ok(TimeSlot);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPost("{ServiceId}")]
		public IActionResult AddTimeSlots(int ServiceId, [FromBody] ICollection<TimeSlotDto> timeSlots)
		{
			try
			{
				if (!ModelState.IsValid || timeSlots == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (timeSlots.Count > 3)
				{
					return BadRequest(ApiResponse.TimeSlotsExceededMax);
				}
				if (!_requestRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				List<TimeSlot> mapTimeSlots = new List<TimeSlot>();
				foreach (var item in timeSlots)
				{
					var mapItem = _mapper.Map<TimeSlot>(item);
					mapItem.ServiceRequest = _requestRepository.GetService(ServiceId);
					mapTimeSlots.Add(mapItem);
				}
				_timeSlotRepository.AddTimeSlots(mapTimeSlots);
				return Ok(ApiResponse.SuccessCreated);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut("{ServiceId}")]
		public IActionResult UpdateTimeSlots(int ServiceId, [FromBody] ICollection<TimeSlotDto> timeSlots)
		{
			try
			{
				if (!ModelState.IsValid || timeSlots == null)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (timeSlots.Count > 3)
				{
					return BadRequest(ApiResponse.TimeSlotsExceededMax);
				}
				if (!_requestRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponse.RequestNotFound);
				}
				List<TimeSlot> mapTimeSlots = new List<TimeSlot>();
				foreach (var item in timeSlots)
				{
					var mapItem = _mapper.Map<TimeSlot>(item);
					mapItem.ServiceRequest = _requestRepository.GetService(ServiceId);
					mapTimeSlots.Add(mapItem);
				}
				_timeSlotRepository.UpdateTimeSlots(mapTimeSlots, ServiceId);
				return Ok(ApiResponse.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}


		//[HttpDelete("{TimeSlotId}")]
		//[ProducesResponseType(204)]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		//public IActionResult DeleteTimeSlot(int TimeSlotId)
		//{
		//	if (!_timeSlotRepository.TimeSlotExist(TimeSlotId))
		//	{
		//		return NotFound(ApiResponse.TimeSlotNotFound);
		//	}
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ApiResponse.NotValid);
		//	}

		//	if (!_timeSlotRepository.DeleteTimeSlot(TimeSlotId))
		//	{
		//		return StatusCode(500, ApiResponse.SomethingWrong);
		//	}
		//	return Ok(ApiResponse.SuccessDeleted);
		//}
	}
}
