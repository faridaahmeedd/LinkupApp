using ServicesApp.Models;

namespace ServicesApp.Core.Models
{
	public class Customer
	{
        public int Id { get; set; }
		public required String Email { get; set; }
		public required String Password { get; set; }
		public required String FName { get; set; }
		public required String LName { get; set; }
		public required String PhoneNumber { get; set; }
		public required String City { get; set; }
		public required String Country { get; set; }
		public required bool Gender{ get; set; }
		public DateOnly BirthDate{ get; set; }
		public ICollection<Service> services { get; set; }
	}
}
