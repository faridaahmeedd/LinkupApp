using ServicesApp.Core.Models;

namespace ServicesApp.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public required string ReporterRole { get; set; }
		public required string ReporterName { get; set; }
		public required ServiceRequest Request { get; set; }
    }
}
