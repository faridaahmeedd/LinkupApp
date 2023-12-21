using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
	public class ServiceOfferRepository : IServiceOfferRepository
	{
		private readonly DataContext _context;

		public ServiceOfferRepository(DataContext context)
		{
			this._context = context;
		}

		public ICollection<ServiceOffer> GetOffers()
		{
			return _context.Offers.OrderBy(p => p.Id).ToList();
		}

		public ServiceOffer GetOffer(int id)
		{
			return _context.Offers.Where(p => p.Id == id).FirstOrDefault();
		}

		public bool OfferExist(int id)
		{
			return _context.Offers.Any(p => p.Id == id);
		}

		public bool CreateOffer(ServiceOffer offer)
		{
			_context.Add(offer);
			return Save();
		}
		public bool AcceptOffer(int id)
		{
			var existingOffer = _context.Offers.Find(id);
			Console.WriteLine(existingOffer);
			if (existingOffer != null)
			{
				existingOffer.Accepted = true;

				_context.SaveChanges();
				return true;
			}
			return false;
		}

		public bool UpdateOffer(ServiceOffer updatedOffer)
		{
			var existingOffer = _context.Offers.Find(updatedOffer.Id);
			Console.WriteLine(existingOffer);
			if (existingOffer != null)
			{
				existingOffer.Fees = updatedOffer.Fees;
				existingOffer.TimeSlotId = updatedOffer.TimeSlotId;

				_context.SaveChanges();
				return true;
			}
			return false;
		}

		public bool DeleteOffer(int id)
		{
			var offer = _context.Offers.Where(p => p.Id == id).FirstOrDefault();
			_context.Remove(offer!);
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
