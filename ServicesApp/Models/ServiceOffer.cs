namespace ServicesApp.Models
{
    public class ServiceOffer
	{
		public int Id { get; set; }
		public required int Fees { get; set; }
		public bool Accepted { get; set; } = false;
		public required int TimeSlotId { get; set; }
		public required TimeOnly Duration { get; set; }
		public required Provider Provider { get; set; }
        public required ServiceRequest Request { get; set; }
	}
}