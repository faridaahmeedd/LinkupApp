﻿namespace ServicesApp.Dto.Service
{
    public class PostServiceRequestDto
    {
        public required string Description { get; set; }
        public string Location { get; set; }
		public string PaymentMethod { get; set; }
		public bool Volunteer { get; set; }
        public byte[]? Image { get; set; }
    }
}