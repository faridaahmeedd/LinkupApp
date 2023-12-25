using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<Customer> _userManager;

		public CustomerRepository(DataContext context, UserManager<Customer> userManager)
        {
            _context = context;
			_userManager = userManager;
		}
        public bool CustomerExist(string id)
		{
			return _context.Customers.Any(p => p.Id == id);
		}

		public Customer GetCustomer(string id)
		{
			return _context.Customers.FirstOrDefault(p => p.Id == id);
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

		public async Task<IdentityResult> UpdateCustomer(Customer customerUpdate)
		{
			var existingCustomer = await _userManager.FindByIdAsync(customerUpdate.Id);
			existingCustomer.FName = customerUpdate.FName;
			existingCustomer.LName = customerUpdate.LName;
			existingCustomer.Address = customerUpdate.Address;
			existingCustomer.City = customerUpdate.City;
			existingCustomer.Country = customerUpdate.Country;
			existingCustomer.BirthDate = customerUpdate.BirthDate;
			existingCustomer.Gender = customerUpdate.Gender;
			existingCustomer.Disability = customerUpdate.Disability;
			existingCustomer.EmergencyContact = customerUpdate.EmergencyContact;
			existingCustomer.MobileNumber = customerUpdate.MobileNumber;
			var result = await _userManager.UpdateAsync(existingCustomer);
			return result;
		}

		public async Task<IdentityResult> DeleteCustomer(string id)
		{
            var customer = _context.Customers.Include(c => c.Services).SingleOrDefault(c => c.Id == id);

            if (customer != null)
            {
                // Remove related requests
                //_context.Requests.RemoveRange(customer.Services);
				foreach (var service in customer.Services)
				{
					if(service.Status == "Requested")
					{
						_context.Remove(service);
					}
					service.Customer = null;
				}
				// Save changes
                await _context.SaveChangesAsync();

                // Delete the customer
                var _customer = await _userManager.FindByIdAsync(id);
                var result = await _userManager.DeleteAsync(_customer);

                return result;
            }
			return null;
        }

        

        public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
