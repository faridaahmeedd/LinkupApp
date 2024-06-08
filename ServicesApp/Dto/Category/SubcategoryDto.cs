namespace ServicesApp.Dto.Subcategory
{
    public class SubcategoryDto
    {
        public int Id { get; set; }
        public required string NameEn { get; set; }
        public required string DescriptionEn { get; set; }
        public required string NameAr { get; set; }
        public required string DescriptionAr { get; set; }
        public required int MinFeesEn { get; set; }
        public required int MaxFeesEn { get; set; }
        public required string MinFeesAr { get; set; }
        public required string MaxFeesAr { get; set; }
    }
}
