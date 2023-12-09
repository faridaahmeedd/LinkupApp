namespace ServicesApp.Models
{
	public class Provider
	{
		public int Id { get; set; }
		public required String Email { get; set; }
		public required String Password { get; set; }
		public required String FName { get; set; }
		public required String LName { get; set; }
		public required String PhoneNumber { get; set; }
		public required String City { get; set; }
		public required String Country { get; set; }
		public required bool Gender { get; set; }
		public DateOnly BirthDate { get; set; }
        public required String JobTitle { get; set; }
        public String? Description { get; set; }
        public ICollection<ServiceOffer>? Offers { get; set; }
		// SKILLS
	}
}
