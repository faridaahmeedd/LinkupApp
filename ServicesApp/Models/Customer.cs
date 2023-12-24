using ServicesApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicesApp.Core.Models
{
	[Table("Customer")]
	public class Customer : AppUser
	{
		private String? _FName;
        private String? _LName;
        private String? _City;
        private String? _Country;
        private String? _Address;
        private bool? _Gender;
        private DateOnly? _BirthDate;
        private String? _Disability;
        private String? _EmergencyContact;
        private ICollection<ServiceRequest>? _Services;

        public String? FName
        {
            get { return _FName; }
            set { _FName = value; }
        }
        public string? LName
        {
            get { return _LName; }
            set { _LName = value; }
        }
        public String? City
        {
            get { return _City; }
            set { _City = value; }
        }
        public String? Country
        {
            get { return _Country; }
            set { _Country = value; }
        }
        public String? Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        public bool? Gender
        {
            get { return _Gender; }
            set { _Gender = value; }
        }
        public DateOnly? BirthDate
        {
            get { return _BirthDate; }
            set { _BirthDate = value; }
        }
        public String? Disability
        {
            get { return _Disability; }
            set { _Disability = value; }
        }
        public String? EmergencyContact
        {
            get { return _EmergencyContact; }
            set { _EmergencyContact = value; }
        }
        public ICollection<ServiceRequest>? Services
        {
           get { return _Services; }
           set { _Services = value; } 
        }
    }
}
