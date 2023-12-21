using Microsoft.EntityFrameworkCore;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceOfferRepository
	{
		public ICollection<ServiceOffer> GetOffers();
		public ServiceOffer GetOffer(int id);
		public bool OfferExist(int id);
		public bool CreateOffer(ServiceOffer offer);
		public bool UpdateOffer(ServiceOffer updatedOffer);
		public bool DeleteOffer(int id);
		public bool AcceptOffer(int id);
		public bool Save();
	}
}
