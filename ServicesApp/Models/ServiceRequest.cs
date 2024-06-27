using ServicesApp.Models;

namespace ServicesApp.Models
{
    public class ServiceRequest
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public string Status { get; set; } = "Requested" ;      // Requested Pending Completed
		public string? Location { get; set; }
		public required string PaymentMethod { get; set; }
		public string PaymentStatus { get; set; } = "Pending";  // Pending Paid
		public bool Volunteer { get; set; } = false;
		public string? ExaminationComment { get; set; }
		public Subcategory? Subcategory { get; set; }
        public Customer? Customer { get; set; }
		public ICollection<Image>? Images { get; set; }
		public ICollection<ServiceOffer>? Offers { get; set; }
		public ICollection<TimeSlot> TimeSlots { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}