namespace ServicesApp.Models
{
    public class ServiceOffer
	{
		public int Id { get; set; }
		public int Fees { get; set; }
		public bool Accepted { get; set; } = false;
		public required Provider Provider { get; set; }
        public required ServiceRequest Request { get; set; }
        public required int TimeSlotId { get; set; }
    }
}
