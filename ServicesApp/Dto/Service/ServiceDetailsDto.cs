using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceDetailsDto
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public int? MaxFees { get; set; }
        //public byte[]? Image { get; set; }
        public ICollection<ServiceOfferDto>? Offers { get; set; }
        public required ICollection<TimeSlotDto> TimeSlots { get; set; }
    }
}
