namespace ServicesApp.Dto.Service
{
	public class GetCalendarDto
	{
		public int RequestId { get; set; }
		public int OfferId { get; set; }
		public required string Date { get; set; }
		public required string FromTime { get; set; }
		public required string ToTime { get; set; }
		public string SubcategoryNameAr { get; set; }
        public string SubcategoryNameEn { get; set; }

    }
}
