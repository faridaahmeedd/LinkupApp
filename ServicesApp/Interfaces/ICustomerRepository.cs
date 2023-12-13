using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICustomerRepository 
	{
		ICollection<Customer> GetCustomers();
		Customer GetCustomer(string id);
		// Customer GetCustomer(string email, string password);
		bool CustomerExist(string id);
		ICollection<ServiceRequest> GetServicesByCustomer(string id);
		bool CreateCustomer(Customer customer);
		bool UpdateCustomer(Customer customer);
		bool DeleteCustomer(string id);
		bool Save();
	}
}
