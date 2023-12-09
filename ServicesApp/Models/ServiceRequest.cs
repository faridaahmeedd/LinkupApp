using ServicesApp.Core.Models;

namespace ServicesApp.Models
{
	public class ServiceRequest
	{
		public int Id { get; set; }
		public required string Description { get; set; }
        public int Fees { get; set; }
		public required List<Tuple<DateTime, DateTime>> TimeSlots { get; set; }
		public IFormFile? Image { get; set; }
		public required Category Category { get; set; }
		public required Customer Customer { get; set; }
		public ICollection<ServiceOffer>? Offers { get; set; }

	}
}