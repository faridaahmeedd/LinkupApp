namespace ServicesApp.Dto.Service
{
	public class GetServiceOfferDto
	{
		public int Id { get; set; }
		public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
		public required string Duration { get; set; }
		public string Status { get; set; }
		public string ProviderId { get; set; }
		public string ProviderName { get; set; }
        public double ProviderAvgRating { get; set; }
        public int RequestId { get; set; }
        public required bool Examination { get; set; }
		public bool AdminOffer { get; set; } = false;
		public string? AdminOfferStatus { get; set; }  // approved declined
	}
}