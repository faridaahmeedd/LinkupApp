using ServicesApp.Core.Models;

namespace ServicesApp.Models
{
	public class Review
	{
		public int Id { get; set; }
		public int? Rate { get; set; }
		public string? Comment { get; set; }
		//public required string ReviewerName { get; set; }
		public required string ReviewerRole { get; set; }
		public required ServiceRequest request { get; set; }
	}
}