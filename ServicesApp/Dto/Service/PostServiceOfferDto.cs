namespace ServicesApp.Dto.Service
{
    public class PostServiceOfferDto
    {
		//public int Id { get; set; }
		public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
        public required string Duration { get; set; }
		//public string Status { get; set; } = "Offered";
		//public bool Accepted { get; set; }
		//public required int RequestId { get; set; }
		//public required string ProviderId { get; set; }
	}
}
