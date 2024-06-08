namespace ServicesApp.Dto.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string NameEn { get; set; }
        public required string DescriptionEn { get; set; }
        public required string NameAr { get; set; }
        public required string DescriptionAr { get; set; }
    }
}
