namespace ServicesApp.Models
{
	public class Subcategory
	{
		public int Id { get; set; }
        public required string NameEn { get; set; }
        public required string DescriptionEn { get; set; }
        public required string NameAr { get; set; }
        public required string DescriptionAr { get; set; }
        public required int MinFeesEn { get; set; }
		public required int MaxFeesEn { get; set; }
        public required int MinFeesAr { get; set; }
        public required int MaxFeesAr { get; set; }
        public required Category Category { get; set; }
		public ICollection<ServiceRequest>? Services { get; set; }
	}
}