﻿using ServicesApp.Core.Models;
using System.Text.Json.Serialization;

namespace ServicesApp.Models
{
    public class ServiceRequest
	{
		public int Id { get; set; }
		public required string Description { get; set; }
        public int Fees { get; set; }
		public byte[]? Image { get; set; }
		public required Category Category { get; set; }
		public required Customer Customer { get; set; }
		public ICollection<ServiceOffer>? Offers { get; set; }
		public required ICollection<TimeSlot> TimeSlots { get; set; }
	}
}