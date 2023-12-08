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
	}
}
