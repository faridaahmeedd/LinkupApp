using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ServicesApp.Models
{
	[Table("Provider")]
	public class Provider : AppUser
	{
		public required String FName { get; set; }
		public required String LName { get; set; }
		public required String City { get; set; }
		public required String Country { get; set; }
		public required String Address { get; set; }
		public required bool Gender { get; set; }
		public required DateOnly BirthDate { get; set; }
		public required String JobTitle { get; set; }
        public String? Description { get; set; }
		public ICollection<ServiceOffer>? Offers { get; set; }
		// SKILLS
	}
}
