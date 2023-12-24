using System.ComponentModel.DataAnnotations.Schema;

namespace ServicesApp.Models
{
	public class Admin 
	{
<<<<<<< HEAD
        private string _Id;
        private String _Password;
        private string _Email;
        
        public Admin()
        {
            _Id = string.Empty;
            _Password = string.Empty;
            _Email = string.Empty;
        }
        
        public string Id
        { 
            get { return _Id; }
            set { _Id = value; }
        }
       
        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        
=======
        public string Id { get; set; }
        public String Password { get; set; }
        public string Email { get; set; }
   
>>>>>>> 202b1ab411ef1c7768f862d4e28eaee6224ad19f
    }
}
