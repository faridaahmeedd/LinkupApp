using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        //public byte[]? Image { get; set; }
        
        //public string? CategoryName { get; set; }
        //public required string CustomerName { get; set; }
        //public required ICollection<TimeSlotDto> TimeSlots { get; set; }  list mn from 
        //public required Dictionary<DateOnly, Tuple<TimeOnly, TimeOnly>> TimeSlots { get; set; }
    }
}
