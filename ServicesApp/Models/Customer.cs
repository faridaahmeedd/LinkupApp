using ServicesApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ServicesApp.Core.Models
{
	[Table("Customer")]
	public class Customer : AppUser
	{
        public String? FName { get; set; }
		public String? LName { get; set; }
		public String? City { get; set; }
		public String? Country { get; set; }
        public String? Address { get; set; }
        public String? MobileNumber { get; set; }
        public bool Gender { get; set; }
		public DateOnly BirthDate { get; set; }
		public String? Disability { get; set; }
        public String? EmergencyContact { get; set; }
		public int Balance { get; set; }
        public byte[]? Image { get; set; }
        public ICollection<ServiceRequest>? Services { get; set; }
	}
}
