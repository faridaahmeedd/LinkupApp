namespace ServicesApp.Dto
{
	public class CustomerDto
	{
		public required String FName { get; set; }
		public required String LName { get; set; }
		public required String City { get; set; }
		public required String Country { get; set; }
		public required String Address { get; set; }
		public required bool Gender { get; set; }
		public required DateOnly BirthDate { get; set; }
		public String? Disability { get; set; }
		public String? EmergencyContact { get; set; }
	}
}
