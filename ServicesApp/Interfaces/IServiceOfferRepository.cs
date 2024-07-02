using Microsoft.EntityFrameworkCore;
using ServicesApp.Dto.Service;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceOfferRepository
	{
		ICollection<ServiceOffer> GetOffers();
		ServiceOffer GetOffer(int id);
		bool OfferExist(int id);
		bool CreateOffer(ServiceOffer offer);
		bool AdminAssignProvider(int offerId, string providerId);
		bool ApproveAdminOffer(int offerId);
		bool UpdateOffer(ServiceOffer updatedOffer);
		bool DeleteOffer(int id);
		bool AcceptOffer(int id);
		bool IsOfferAccepted(int requestId);
        bool DeclineOffer(int offerId);
		ICollection<ServiceOffer> GetUnCompletedOffers(string providerId);
		ICollection<ServiceOffer> GetOfffersOfProvider(string providerId);
		bool ProviderAlreadyOffered(string providerId, int requestId);
		bool CheckFeesRange(ServiceOffer serviceOffer);
		ICollection<GetCalendarDto> GetCalendarDetails(string ProviderId);
        bool Save();
	}
}
