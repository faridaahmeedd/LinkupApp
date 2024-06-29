using Microsoft.AspNetCore.Identity;
using ServicesApp.Models;
using System.Threading.Tasks;

namespace ServicesApp.Interfaces
{
	public interface IProviderRepository
	{
		ICollection<Provider> GetProviders();
		Provider GetProvider(string id);
		bool ProviderExist(string id);
		ICollection<ServiceOffer> GetOffersByProvider(string id);
		Task<bool> UpdateProvider(Provider ProviderUpdate);
		Task<bool> DeleteProvider(string id);
		bool CheckProviderBalance(string id);
		Task<bool> ApproveProvider(string id);
		bool CheckApprovedProvider(string id);
		bool Save();
	}
}
