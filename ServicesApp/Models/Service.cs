using ServicesApp.Core.Models;

namespace ServicesApp.Models
{
	public class Service
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
        public int Fees { get; set; }
		public required Category Category { get; set; }
		public required Customer Customer { get; set; }
	}
}
