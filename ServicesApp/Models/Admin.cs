using System.ComponentModel.DataAnnotations.Schema;

namespace ServicesApp.Models
{
	public class Admin 
	{
        public string Id { get; set; }
        public String Password { get; set; }
        public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
    }
}
