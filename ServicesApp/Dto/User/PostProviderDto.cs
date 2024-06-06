namespace ServicesApp.Dto.User
{
    public class PostProviderDto
    {
        public required string FName { get; set; }
        public required string LName { get; set; }
        public required string MobileNumber { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string Address { get; set; }
        public required bool Gender { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required string JobTitle { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
    }
}
