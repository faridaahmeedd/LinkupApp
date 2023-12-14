using ServicesApp.Models;

namespace ServicesApp.Dto.Service
{
    public class ServiceOfferDto
    {
        public int Id { get; set; }
        public required string ProviderName { get; set; }
        public int Fees { get; set; }
        public int RequestId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
        public bool Accepted { get; set; }
    }
}
