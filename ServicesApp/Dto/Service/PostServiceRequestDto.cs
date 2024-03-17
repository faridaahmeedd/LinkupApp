namespace ServicesApp.Dto.Service
{
    public class PostServiceRequestDto
    {
       // public int Id { get; set; }
        public required string Description { get; set; }
        public string Location { get; set; }
		public string PaymentMethod { get; set; }
	    public byte[]? Image { get; set; }
		//public int MaxFees { get; set; } = 0;
	}
}