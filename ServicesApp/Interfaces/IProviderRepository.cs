using Microsoft.AspNetCore.Identity;
using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IProviderRepository
	{
		ICollection<Provider> GetProviders();
		Provider GetProvider(string id);
		bool ProviderExist(string id);
		ICollection<ServiceOffer> GetOffersByProvider(string id);
		bool CreateProvider(Provider Provider);
		Task<IdentityResult> UpdateProvider(Provider ProviderUpdate);
		Task<IdentityResult> DeleteProvider(string id);
		bool Save();
	}
}
