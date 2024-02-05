using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public string Location { get; set; }
        //public byte[]? Image { get; set; }
    }
}
