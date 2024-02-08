namespace ServicesApp.Dto.Service
{
    public class TimeSlotDto
    {
		public int Id { get; set; }
		public required string Date { get; set; }
        public required string FromTime { get; set; }
        //public required TimeOnly ToTime { get; set; }
    }
}
