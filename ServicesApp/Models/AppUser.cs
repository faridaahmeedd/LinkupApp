﻿using Microsoft.AspNetCore.Identity;

namespace ServicesApp.Models
{
	public class AppUser : IdentityUser
	{
		// identity user already have id, email, password, phoneNumber
		public bool Active { get; set; } = true;
	}
}
