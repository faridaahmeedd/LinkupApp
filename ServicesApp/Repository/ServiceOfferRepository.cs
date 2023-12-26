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
			//var existingOffer = _context.Offers.Find(id);
            var existingOffer = _context.Offers.Include(o => o.Request).FirstOrDefault(o => o.Id == id);
            Console.WriteLine(existingOffer);
			if (existingOffer != null)
			{
				existingOffer.Accepted = true;
				Console.WriteLine($"offer {existingOffer}");
                Console.WriteLine($"request {existingOffer.Request}");
                var service = _context.Requests.Find(existingOffer.Request.Id);
				service.Status = "Pending";
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
		//request -- offer req---offer delete (accepted & in same day)
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

		public ICollection<ServiceOffer> GetPendingOffers(string providerId)
		{
			var offers = _context.Offers.Include(o => o.Request).Where(p => p.Provider.Id == providerId).ToList(); ;

			if (offers != null)
			{
				var pendingOffers = offers.Where(o => o.Request.Status == "Pending").ToList();
				return pendingOffers;
			}

			return null;
		}
	}
}
