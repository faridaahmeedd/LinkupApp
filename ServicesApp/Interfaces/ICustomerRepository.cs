using Microsoft.AspNetCore.Identity;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICustomerRepository 
	{
		ICollection<Customer> GetCustomers();
		Customer GetCustomer(string id);
		bool CustomerExist(string id);
		Task<bool> UpdateCustomer(Customer customerUpdate);
		Task<bool> DeleteCustomer(string id);
		bool CheckCustomerBalance(string id);
        bool Save();
	}
}
