namespace ServicesApp.Dto.Service
{
	public class GetServiceRequestDto
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public string Location { get; set; }
		public string Status { get; set; } = "Requested";
		public string ProviderName { get; set; }
		public string ProviderMobileNumber { get; set; }
		public byte[]? Image { get; set; }
        public string SubCategoryName { get; set; }
    }
}
