﻿namespace ServicesApp.Models
{
	public class Category
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public string? Description { get; set; }
        public int MinFees { get; set; }
    }
}
