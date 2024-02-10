﻿using Microsoft.EntityFrameworkCore;
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
			return _context.Offers.Include(o => o.Request).Where(p => p.Id == id).FirstOrDefault();
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
		public bool CheckMaxFees(ServiceOffer  serviceOffer)
		{
			var request = _context.Requests.Where(r=> r.Id == serviceOffer.Request.Id).FirstOrDefault();//error
			if(request != null)
			{
				if(request.MaxFees >= serviceOffer.Fees ) {
					return true;
				}
			}
            return false;
		}

		public bool AcceptOffer(int id)
		{
            var existingOffer = _context.Offers.Include(o => o.Request).FirstOrDefault(o => o.Id == id);
			if (existingOffer != null)
			{
				existingOffer.Status = "Accepted";
                var service = _context.Requests.Find(existingOffer.Request.Id);
				service.Status = "Offered";
                _context.SaveChanges();
				return true;
			}
			return false;
		}

        public bool DeclineOffer(int offerId)
        {
            var serviceOffer = _context.Offers.FirstOrDefault(o => o.Id == offerId);

            if (serviceOffer != null)
            {
				serviceOffer.Status = "Declined";
                return Save();
            }
            return false;
        }

        public bool UpdateOffer(ServiceOffer updatedOffer)
		{
			var existingOffer = _context.Offers.Find(updatedOffer.Id);
			if (existingOffer != null )
			{
				if(existingOffer.Status != "Accepted")
				{
					if (existingOffer.Fees != updatedOffer.Fees)
					{
						existingOffer.Status = "Offered";
					}
					existingOffer.Fees = updatedOffer.Fees;
					existingOffer.TimeSlotId = updatedOffer.TimeSlotId;
					existingOffer.Duration = updatedOffer.Duration;
					return Save();
				}
			}
			return false;
		}

		public bool DeleteOffer(int id)
		{
			var offer = _context.Offers.Include(c => c.Provider).Include(c => c.Request).Where(p => p.Id == id).FirstOrDefault();
			if (offer.Status == "Accepted")
			{
				var timeSlot = _context.TimeSlots.Where(t => t.Id == offer.TimeSlotId).FirstOrDefault();

				DateTime offerTime = timeSlot.Date.ToDateTime(timeSlot.FromTime);
				DateTime TimeAfter24 = DateTime.Now.AddHours(24);
				TimeSpan timeDifference = TimeAfter24 - offerTime;

				// Check if the difference is greater than or equal to 24 hours
				if (offerTime <= TimeAfter24)
				{
					offer.Provider.Balance += (offer.Fees * 10) / 100;
				}
				offer.Request.Status = "Requested";
				timeSlot.ToTime = TimeOnly.MinValue;
			}
			_context.Remove(offer);
			return Save();
		}

		public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public ICollection<ServiceOffer> GetUnCompletedOffers(string providerId)
		{
			var offers = _context.Offers.Include(o => o.Request).Where(p => p.Provider.Id == providerId).ToList(); ;

			if (offers != null)
			{
				var pendingOffers = offers.Where(o => o.Request.Status != "Completed").ToList();
				return pendingOffers;
			}

			return null;
		}
        public ICollection<ServiceOffer> GetOfffersOfProvider(string providerId)
		{
            var offers = _context.Offers.Include(o=> o.Request).Where(p => p.Provider.Id == providerId).ToList(); 
            return offers;
        }

        public bool ProviderAlreadyOffered(string providerId , int requestId)
        {
			var providerOffers = GetOfffersOfProvider(providerId);
            if (providerOffers != null)
			{
				foreach (var offer in providerOffers)
                {
                    if (offer.Request.Id == requestId)
					{
						return true;
					}
				}
			}
            return false;
        }
    }
}
