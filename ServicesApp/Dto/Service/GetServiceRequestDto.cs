namespace ServicesApp.Dto.Service
{
	public class GetServiceRequestDto
	{
		public int Id { get; set; }
		public required string Description { get; set; }
		public string SubCategoryNameAr { get; set; }
        public string SubCategoryNameEn { get; set; }

        public string Location { get; set; }
		public string Status { get; set; }
		public string CustomerId { get; set; }
		public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }

        public string PaymentMethod { get; set; }
		public string PaymentStatus { get; set; }
		public bool Volunteer { get; set; }
		public byte[]? Image { get; set; }
		//public int MaxFees { get; set; }
		//public int MinFees { get; set; }
	}
}
