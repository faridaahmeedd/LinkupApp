namespace ServicesApp.Dto.Service
{
	public class GetServiceOfferDto
	{
		public int Id { get; set; }
		public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
		public required string Duration { get; set; }
		public string Status { get; set; } = "Offered";
		public string CustomerName { get; set; }
		public string CustomerMobileNumber { get; set; }

        //public required int RequestId { get; set; }
    }
}