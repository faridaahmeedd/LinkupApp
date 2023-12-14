using Microsoft.AspNetCore.Identity;
using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICustomerRepository 
	{
		ICollection<Customer> GetCustomers();
		Customer GetCustomer(string id);
		bool CustomerExist(string id);
		ICollection<ServiceRequest> GetServicesByCustomer(string id);
		bool CreateCustomer(Customer customer);
		Task<IdentityResult> UpdateCustomer(Customer customerUpdate);
		Task<IdentityResult> DeleteCustomer(string id);
		bool Save();
	}
}
