using ServicesApp.Core.Models;

namespace ServicesApp.Models
{
	public class Review
	{
		public int Id { get; set; }
		public int? Rate { get; set; }
		public string? Comment { get; set; }
		public required string ReviewerId { get; set; }
		public required Provider Provider { get; set; }
		public required Customer Customer { get; set; }
	}
}