using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Dto
{
	public class ServiceRequestDto
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public int Fees { get; set; }
		public byte[]? Image { get; set; }
		public string? CategoryName { get; set; }
		public required String CustomerName { get; set; }
		public required Dictionary<DateOnly, Tuple<TimeOnly, TimeOnly>> TimeSlots { get; set; }

        public ServiceRequestDto()
        {
			TimeSlots = new Dictionary<DateOnly, Tuple<TimeOnly, TimeOnly>>();
		}
    }
}
