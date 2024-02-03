using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
	public class ProviderRepository : IProviderRepository
	{
		public readonly DataContext _context;
		private readonly UserManager<Provider> _userManager;

		public ProviderRepository(DataContext context, UserManager<Provider> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public bool ProviderExist(string id)
		{
			return _context.Providers.Any(p => p.Id == id);
		}

		public Provider GetProvider(string id)
		{
			return _context.Providers.FirstOrDefault(p => p.Id == id);
		}

		public ICollection<Provider> GetProviders()
		{
			return _context.Providers.OrderBy(p => p.Id).ToList();
		}

		public ICollection<ServiceOffer> GetOffersByProvider(string id)
		{
			return _context.Offers.Where(p => p.Provider.Id == id).ToList();
		}


		public async Task<bool> UpdateProvider(Provider ProviderUpdate)
		{
			var existingProvider = await _userManager.FindByIdAsync(ProviderUpdate.Id);
			existingProvider.FName = ProviderUpdate.FName;
			existingProvider.LName = ProviderUpdate.LName;
			existingProvider.Address = ProviderUpdate.Address;
			existingProvider.City = ProviderUpdate.City;
			existingProvider.Country = ProviderUpdate.Country;
			existingProvider.BirthDate = ProviderUpdate.BirthDate;
			existingProvider.Gender = ProviderUpdate.Gender;
			existingProvider.JobTitle = ProviderUpdate.JobTitle;
			existingProvider.Description = ProviderUpdate.Description;
			existingProvider.MobileNumber = ProviderUpdate.MobileNumber;
		//	existingProvider.Image = ProviderUpdate.Image;
            var result = await _userManager.UpdateAsync(existingProvider);
			return result.Succeeded;
		}

        public async Task<bool> DeleteProvider(string id)
        {
            var provider = await _userManager.FindByIdAsync(id);
            // Delete unaccepted offers
            var offers = _context.Offers.Include(o => o.Provider).Where(o => o.Provider.Id == id && o.Status != "Accepted").ToList();
            if (offers != null)
            {
                _context.RemoveRange(offers);
                _context.SaveChanges();
            }
            var result = await _userManager.DeleteAsync(provider);
            return result.Succeeded;
        }
		
        public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

        public bool CheckProviderBalance(string id)
        {
            var existingProvider = _context.Providers.Where(p => p.Id == id).FirstOrDefault();
            if (existingProvider.Balance > 0)
            {
                return false;
            }
            return true;
        }
    }
}
