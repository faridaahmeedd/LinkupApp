using System.ComponentModel.DataAnnotations;

namespace ServicesApp.Models
{
	public class Service
	{
		[Required]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
        public int Fees { get; set; }
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }

    }
}
