﻿namespace ServicesApp.Models
{
	public class Category
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
        public ICollection<ServiceRequest>? Services { get; set; }
    }
}