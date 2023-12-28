namespace ServicesApp.Dto.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int MinFees { get; set; }
    }
}
