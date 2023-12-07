using System.ComponentModel.DataAnnotations;

namespace ServicesApp.Models
{
	public class Service
	{
		public required int Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
        public int Fees { get; set; }
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
		public required Category Category { get; set; }
	}
}
