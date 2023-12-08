using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
	public class CustomerRepository : ICustomerRepository
	{
		public readonly DataContext _context;
        public CustomerRepository(DataContext context)
        {
            _context = context;
        }
        public bool CustomerExist(int id)
		{
			return _context.Customers.Any(p => p.Id == id);
		}

		public Customer GetCustomer(int id)
		{
			return _context.Customers.Where(p => p.Id == id).FirstOrDefault();
		}

		public Customer GetCustomer(string Fname)
		{
			return _context.Customers.Where(p => p.FName == Fname).FirstOrDefault();
		}

		public ICollection<Customer> GetCustomers()
		{
			return _context.Customers.OrderBy(p => p.Id).ToList();
		}

		public ICollection<Service> GetServicesByCustomer(int id)
		{
			return _context.Services.Where(p => p.Customer.Id == id).ToList();
		}
	}
}
