using System.ComponentModel.DataAnnotations.Schema;


namespace ServicesApp.Models
{
	[Table("Provider")]
	public class Provider : AppUser
	{
        private String? _FName;
        private String? _LName;
        private String? _City;
        private String? _Country;
        private String? _Address;
        private bool? _Gender;
        private DateOnly? _BirthDate;
        private String? _JobTitle;
        private String? _Description;
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
        public String? JobTitle
        {
            get { return _JobTitle; }
            set { _JobTitle = value; }
        }
        public String? Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public ICollection<ServiceRequest>? Services
        {
            get { return _Services; }
            set { _Services = value; }
        }

        // SKILLS mmkn akhleha choices (choose category -- choose hwa by3ml eh fih -- zy sabak w bysla7 shtafat w kda)
    }
}
