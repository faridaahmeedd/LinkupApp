﻿using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceOfferDto
    {
		public required int RequestId { get; set; }
		public required string ProviderId { get; set; }
        public required int Fees { get; set; }
		public required int TimeSlotId { get; set; }
		public bool Accepted { get; set; }
    }
}
