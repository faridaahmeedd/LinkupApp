using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICustomerRepository 
	{
		ICollection<Customer> GetCustomers();
		Customer GetCustomer(int id);
		Customer GetCustomer(string email, string password);
		bool CustomerExist(int id);
		ICollection<ServiceRequest> GetServicesByCustomer(int id);
		bool CreateCustomer(Customer customer);
		bool UpdateCustomer(Customer customer);
		bool DeleteCustomer(int id);
		bool Save();
	}
}
