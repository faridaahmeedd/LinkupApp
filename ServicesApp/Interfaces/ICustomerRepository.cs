using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICustomerRepository 
	{
		ICollection<Customer> GetCustomers();
		Customer GetCustomer(int id);
		Customer GetCustomer(string Fname);
		bool CustomerExist(int id);
		ICollection<Service> GetServicesByCustomer(int id);
		bool CreateCustomer(Customer customer);
		bool UpdateCustomer(Customer customer);
		bool DeleteCustomer(int id);
		bool Save();
	}
}
