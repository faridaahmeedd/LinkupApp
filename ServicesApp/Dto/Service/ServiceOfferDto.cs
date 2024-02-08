using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceOfferDto
    {
		public int Id { get; set; }
		public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
        public required TimeOnly Duration { get; set; }
		//public bool Accepted { get; set; }
		//public required int RequestId { get; set; }
		//public required string ProviderId { get; set; }
	}
}
