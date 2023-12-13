using System.ComponentModel.DataAnnotations.Schema;

namespace ServicesApp.Models
{
    public class TimeSlot
    {
        public int id { get; set; }
        public required DateOnly Date { get; set; }
        public required TimeOnly FromTime { get; set; }
        public required TimeOnly ToTime { get; set; }
        [ForeignKey("ServiceRequest")]
		public required int ServiceRequestId { get; set; }
		public required ServiceRequest ServiceRequest { get; set; }
    }
}
