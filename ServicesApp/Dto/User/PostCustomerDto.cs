﻿namespace ServicesApp.Dto.User
{
    public class PostCustomerDto
    {
        public required string FName { get; set; }
        public required string LName { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string MobileNumber { get; set; }
        public required string Address { get; set; }
        public required bool Gender { get; set; }
        public required DateOnly BirthDate { get; set; }
        public string? Disability { get; set; }
        public string? EmergencyContact { get; set; }
        public byte[]? Image { get; set; }
    }
}
