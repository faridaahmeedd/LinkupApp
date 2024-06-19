namespace ServicesApp.Dto.Service
{
    public class PostServiceOfferDto
    {
		public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
        public required string Duration { get; set; }
        public required bool Examination { get; set; }

    }
}
