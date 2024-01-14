using Microsoft.EntityFrameworkCore;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceOfferRepository
	{
		ICollection<ServiceOffer> GetOffers();
		ServiceOffer GetOffer(int id);
		bool OfferExist(int id);
		bool CreateOffer(ServiceOffer offer);
		bool UpdateOffer(ServiceOffer updatedOffer);
		bool DeleteOffer(int id);
		bool AcceptOffer(int id);
		ICollection<ServiceOffer> GetUnCompletedOffers(string providerId);
		bool Save();
		ICollection<ServiceOffer> GetOfffersOfProvider(string providerId);
		bool ProviderAlreadyOffered(string providerId, int requestId);

    }
}
