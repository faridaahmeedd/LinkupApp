using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ImageDto
    {
        public int Id { get; set; }
        public required byte[] Img { get; set; }
    }
}
