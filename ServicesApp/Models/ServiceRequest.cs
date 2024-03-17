using ServicesApp.Core.Models;

namespace ServicesApp.Models
{
    public class ServiceRequest
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public byte[]? Image { get; set; }
		public string Status { get; set; } = "Requested" ;
		public string? Location { get; set; }
		public required string PaymentMethod { get; set; }
		public string PaymentStatus { get; set; } = "Pending"; //Pending Paid
		public Subcategory? Subcategory { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<ServiceOffer>? Offers { get; set; }
		public required ICollection<TimeSlot> TimeSlots { get; set; }
	}
}