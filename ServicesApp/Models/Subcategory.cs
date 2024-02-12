namespace ServicesApp.Models
{
	public class Subcategory
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required int MinFees { get; set; }
		public required int MaxFees { get; set; }
		public required Category Category { get; set; }
		public ICollection<ServiceRequest>? Services { get; set; }
	}
}