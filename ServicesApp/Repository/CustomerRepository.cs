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
        public bool CustomerExist(string id)
		{
			return _context.Customers.Any(p => p.Id == id);
		}

		public Customer GetCustomer(string id)
		{
			return _context.Customers.Where(p => p.Id == id).FirstOrDefault();
		}

		public ICollection<Customer> GetCustomers()
		{
			return _context.Customers.OrderBy(p => p.Id).ToList();
		}

		public ICollection<ServiceRequest> GetServicesByCustomer(string id)
		{
			return _context.Requests.Where(p => p.Customer.Id == id).ToList();
		}

		public bool CreateCustomer(Customer customer)
		{
			// Change Tracker (add,update,modify)
			_context.Add(customer);
			return Save();

		}

		public bool UpdateCustomer(Customer customer)
		{
			_context.Update(customer);
			return Save();
		}

		public bool DeleteCustomer(string id)
		{
			var cutsomer = _context.Customers.Where(p => p.Id == id).FirstOrDefault();
			_context.Remove(cutsomer!);
			return Save();
		}

		public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
