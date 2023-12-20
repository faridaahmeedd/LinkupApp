using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public int Fees { get; set; }
        //public byte[]? Image { get; set; }

        //public string? CategoryName { get; set; }
        //public required string CustomerName { get; set; }
        //public required ICollection<TimeSlotDto> TimeSlots { get; set; }
        //public required Dictionary<DateOnly, Tuple<TimeOnly, TimeOnly>> TimeSlots { get; set; }
    }
}
