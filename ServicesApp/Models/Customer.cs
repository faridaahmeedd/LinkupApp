using ServicesApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ServicesApp.Core.Models
{
	[Table("Customer")]
	public class Customer : AppUser
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
		public ICollection<ServiceRequest>? Services { get; set; }
	}
}
