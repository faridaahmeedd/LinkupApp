namespace ServicesApp.Models
{
	public class ServiceOffer
	{
		public int Id { get; set; }
		public int Fees { get; set; }
		public Tuple<DateTime, DateTime>? TimeSlot { get; set; }
		public required Provider Provider { get; set; }
        public required ServiceRequest Request { get; set; }
        public bool Accepted { get; set; }
    }
}
