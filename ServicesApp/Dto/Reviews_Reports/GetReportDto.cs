namespace ServicesApp.Dto.Reviews_Reports
{
	public class GetReportDto
	{
		public int Id { get; set; } 
		public string Comment { get; set; }
		public required string ReporterName { get; set; }
		public required int RequestId { get; set; }
	}
}
