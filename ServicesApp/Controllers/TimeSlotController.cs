using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Models;
using ServicesApp.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
		[ProducesResponseType(200, Type = typeof(TimeSlotDto))]
		public IActionResult GetTimeSlot(int TimeSlotId)
		{
			if (!_timeSlotRepository.TimeSlotExist(TimeSlotId))
			{
				return NotFound();
			}
			var TimeSlot = _mapper.Map<TimeSlotDto>(_timeSlotRepository.GetTimeSlot(TimeSlotId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(TimeSlot);
		}


		[HttpGet("service/{ServiceId}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TimeSlotDto>))]
		public IActionResult GetTimeSlotsOfService(int ServiceId)
		{
			if (!_requestRepository.ServiceExist(ServiceId))
			{
				return NotFound();
			}
			var TimeSlot = _mapper.Map<List<TimeSlotDto>>(_timeSlotRepository.GetTimeSlotsOfService(ServiceId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(TimeSlot);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult AddTimeSlot([FromQuery] int ServiceId, [FromBody] ICollection<TimeSlotDto> timeSlots)
		{
			if (timeSlots == null)
			{
				return BadRequest(ModelState);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (!_requestRepository.ServiceExist(ServiceId))
			{
				ModelState.AddModelError("", "Service doesn't exist");
				return StatusCode(422, ModelState);
			}
			List<TimeSlot> mapTimeSlots = new List<TimeSlot>();
			foreach (var item in timeSlots)
			{
				var mapItem = _mapper.Map<TimeSlot>(item);
				mapItem.ServiceRequest = _requestRepository.GetService(ServiceId);
				mapTimeSlots.Add(mapItem);
			}
			if (!_timeSlotRepository.AddTimeSlot(mapTimeSlots))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Created();
		}


		[HttpDelete("{TimeSlotId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult DeleteTimeSlot(int TimeSlotId)
		{
			if (!_timeSlotRepository.TimeSlotExist(TimeSlotId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!_timeSlotRepository.DeleteTimeSlot(TimeSlotId))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully deleted");
		}



		//[HttpPut("update")]
		//[ProducesResponseType(204)]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		//public IActionResult UpdateTimeSlot([FromBody] TimeSlotDto updatedTimeSlotDto , [FromQuery] int TimeSlotId)
		//{
		//    if (updatedTimeSlotDto == null)
		//    {
		//        return BadRequest("Invalid data");
		//    }

		//    // Assuming you have a mapper to map DTO to the entity
		//    var updatedTimeSlot = _mapper.Map<TimeSlot>(updatedTimeSlotDto);
		//    updatedTimeSlot.id = TimeSlotId; 
		//    var success = _timeSlotRepository.UpdateTimeSlot(updatedTimeSlot);

		//    if (success)
		//    {
		//        return NoContent(); // 204 - Success with no content
		//    }
		//    else
		//    {
		//        return NotFound(); // 404 - Not Found
		//    }
		//}
	}
}
