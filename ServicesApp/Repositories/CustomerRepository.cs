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

		public async Task<bool> UpdateCustomer(Customer customerUpdate)
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
			return result.Succeeded;
		}

        public async Task<bool> DeleteCustomer(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            // Delete requests where status = Requested
            var requests = _context.Requests.Include(r => r.Customer).Where(r => r.Customer.Id == id && r.Status == "Requested").ToList();
            if (requests != null)
            {
                _context.RemoveRange(requests);
                _context.SaveChanges();
            }
            var result = await _userManager.DeleteAsync(customer);
            return result.Succeeded;
        }

        public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool CheckCustomerBalance(string id)
		{
			var existingCustomer = _context.Customers.Where(p => p.Id == id).FirstOrDefault();
			if(existingCustomer.Balance >0)
			{
				return false;
			}

            return true;
		}
	}
}
