using Microsoft.AspNetCore.Identity;
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

		public bool CreateProvider(Provider Provider)
		{
			// Change Tracker (add,update,modify)
			_context.Add(Provider);
			return Save();
		}

		public async Task<IdentityResult> UpdateProvider(Provider ProviderUpdate)
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
			var result = await _userManager.UpdateAsync(existingProvider);
			return result;
		}

		public async Task<IdentityResult> DeleteProvider(string id)
		{
			var Provider = await _userManager.FindByIdAsync(id);
			var result = await _userManager.DeleteAsync(Provider);
			return result;
		}

		public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
