using System.ComponentModel.DataAnnotations.Schema;

namespace ServicesApp.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public required DateOnly Date { get; set; }
        public required TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
		public required ServiceRequest ServiceRequest { get; set; }
    }
}
