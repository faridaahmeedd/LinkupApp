using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Service;
using ServicesApp.Models;
using ServicesApp.Interfaces;
using ServicesApp.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotsRepository _timeSlotRepository;
        private readonly IMapper _mapper;

        public TimeSlotsController(ITimeSlotsRepository timeSlotRepository , IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
        }

        [HttpPut("update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTimeSlot([FromBody] TimeSlotDto updatedTimeSlotDto , [FromQuery] int TimeSlotId)
        {
            if (updatedTimeSlotDto == null)
            {
                return BadRequest("Invalid data");
            }

            // Assuming you have a mapper to map DTO to the entity
            var updatedTimeSlot = _mapper.Map<TimeSlot>(updatedTimeSlotDto);
            updatedTimeSlot.id = TimeSlotId; 
            var success = _timeSlotRepository.UpdateTimeSlot(updatedTimeSlot);

            if (success)
            {
                return NoContent(); // 204 - Success with no content
            }
            else
            {
                return NotFound(); // 404 - Not Found
            }
        }
    }
}
