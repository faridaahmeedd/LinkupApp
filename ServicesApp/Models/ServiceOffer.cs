namespace ServicesApp.Models
{
    public class ServiceOffer
	{
		public int Id { get; set; }
		public int Fees { get; set; }
		public bool Accepted { get; set; } = false;
		public int ServiceRequestId { get; set; }
		public int TimeSlotId { get; set; }
		public required Provider Provider { get; set; }
        public required ServiceRequest Request { get; set; }
	}
}
