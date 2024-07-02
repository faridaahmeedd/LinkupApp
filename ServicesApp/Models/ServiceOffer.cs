namespace ServicesApp.Models
{
    public class ServiceOffer
	{
		public int Id { get; set; }
		public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
		public required TimeOnly Duration { get; set; }
		public  Provider? Provider { get; set; }
		public string Status { get; set; } = "Offered";  // declined Accepted
        public required ServiceRequest Request { get; set; }
        public required bool Examination { get; set; }
		public bool AdminOffer { get; set; } = false;
		public string? AdminOfferStatus { get; set; } = null;  // approved declined
	}
}