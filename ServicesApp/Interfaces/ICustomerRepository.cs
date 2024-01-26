﻿using Microsoft.AspNetCore.Identity;
using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICustomerRepository 
	{
		ICollection<Customer> GetCustomers();
		Customer GetCustomer(string id);
		bool CustomerExist(string id);
		Task<IdentityResult> UpdateCustomer(Customer customerUpdate);
		Task<IdentityResult> DeleteCustomer(string id);
		bool CheckCustomerBalance(string id);
        bool Save();
	}
}
