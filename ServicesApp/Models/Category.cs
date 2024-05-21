namespace ServicesApp.Models
{
	public class Category
	{
		public int Id { get; set; }
		public required string NameEn { get; set; }
		public required string DescriptionEn { get; set; }
        public required string NameAr { get; set; }
        public required string DescriptionAr { get; set; }
        public ICollection<Subcategory>? Subcategories { get; set; }
	}
}