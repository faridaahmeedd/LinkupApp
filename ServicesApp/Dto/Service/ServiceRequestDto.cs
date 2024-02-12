namespace ServicesApp.Dto.Service
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public string Location { get; set; }
		public string Status { get; set; } = "Requested";
        public byte[]? Image { get; set; }
        //public int MaxFees { get; set; } = 0;
    }
}