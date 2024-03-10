namespace ServicesApp.Dto.Reviews_Reports
{
	public class GetReviewDto
	{
		public int Id { get; set; }
		public int Rate { get; set; }
		public string? Comment { get; set; }
		public required string ReviewerName { get; set; }
	}
}
