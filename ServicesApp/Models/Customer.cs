using ServicesApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Core.Models
{
	class Customer
	{
		[Required]
        public int id { get; set; }
		[Required]
		public String email { get; set; }
		public String password { get; set; }
		public String fName { get; set; }
		public String lName { get; set; }
		public String phoneNumber { get; set; }
		public String city { get; set; }
		public String country { get; set; }
		public bool gender { get; set; }
		public DateOnly birthDate { get; set; }
		public Category Category { get; set; }
		public ICollection<Service> services { get; set; }
	}
}
