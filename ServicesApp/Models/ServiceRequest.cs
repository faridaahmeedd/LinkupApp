using ServicesApp.Core.Models;
using System.Text.Json.Serialization;

namespace ServicesApp.Models
{
    public class ServiceRequest
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public byte[]? Image { get; set; }
		public string Status { get; set; } = "Requested" ;
		public string? Location { get; set; }
        public int MaxFees { get; set; } = 0;
		public Category? Category { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<ServiceOffer>? Offers { get; set; }
		public required ICollection<TimeSlot> TimeSlots { get; set; }
	}
}