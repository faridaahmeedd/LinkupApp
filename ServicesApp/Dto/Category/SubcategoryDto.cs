namespace ServicesApp.Dto.Subcategory
{
    public class SubcategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int MinFees { get; set; }
		public required int MaxFees { get; set; }
	}
}
