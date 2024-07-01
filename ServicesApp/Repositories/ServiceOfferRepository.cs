﻿using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
	public class ServiceOfferRepository : IServiceOfferRepository
	{
		private readonly DataContext _context;

		public ServiceOfferRepository(DataContext context)
		{
			_context = context;
		}

		public ICollection<ServiceOffer> GetOffers()
		{
			return _context.Offers.Include(p => p.Provider).Include(p => p.Request).OrderBy(p => p.Id).ToList();
		}

		public ServiceOffer GetOffer(int id)
		{
			return _context.Offers.Include(p => p.Provider).Include(p => p.Request).Include(o => o.Request).Where(p => p.Id == id).FirstOrDefault();
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

		public int CreateAdminOffer(int offerId)
		{
			var existingOffer = GetOffer(offerId);

			if (existingOffer != null)
			{
				var newOffer = new ServiceOffer
				{
					Duration = existingOffer.Duration,
					Examination = existingOffer.Examination,
					Fees = existingOffer.Fees,
					Request = existingOffer.Request,
					TimeSlotId = existingOffer.TimeSlotId,
					Status = "Offered",
					AdminOffer = true,
					AdminOfferStatus = null,
				};
				_context.Offers.Add(newOffer);
				Save();
				return newOffer.Id;
			}
			return 0;
		}

		public bool AdminAssignProvider(int offerId, string providerId)
		{
			var existingOffer = GetOffer(offerId);
			if (existingOffer != null && existingOffer.AdminOffer)
			{
				existingOffer.Provider = _context.Providers.Find(providerId);
			}
			return Save();
		}

		public bool ApproveAdminOffer(int offerId)
		{
			var existingOffer = GetOffer(offerId);
			if (existingOffer != null && existingOffer.AdminOffer)
			{
				existingOffer.AdminOfferStatus = "Approved";
				existingOffer.AdminOffer = false;
			}
			return Save();
		}

		public bool CheckFeesRange(ServiceOffer serviceOffer)
		{
			var request = _context.Requests.Include(r => r.Subcategory).Where(r=> r.Id == serviceOffer.Request.Id).FirstOrDefault();
			if (request != null)
			{
				if (request.Volunteer == true || serviceOffer.Examination)
				{
					return true;
				}
				var subCategory = _context.Subcategories.Find(request.Subcategory.Id);
				if (request.Volunteer ||  (subCategory.MaxFeesEn >= serviceOffer.Fees && subCategory.MinFeesEn <= serviceOffer.Fees))
				{
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
				if (IsOfferAccepted(existingOffer.Request.Id))
				{
					return false;
				}
				existingOffer.Status = "Accepted";
                var service = _context.Requests.Find(existingOffer.Request.Id);
				service.Status = "Pending";
                _context.SaveChanges();
				return true;
			}
			return false;
		}
        public bool IsOfferAccepted(int requestId)
        {
            var acceptedOffer = _context.Offers
                                    .Include(o => o.Request)
                                    .FirstOrDefault(o => o.Request.Id == requestId && o.Status == "Accepted");

            return acceptedOffer != null;
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
			if (existingOffer != null)
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
					existingOffer.Examination = updatedOffer.Examination;
					return Save();
				}
			}
			return false;
		}

		public bool DeleteOffer(int id)
		{
			var offer = _context.Offers.Include(c => c.Provider).Include(c => c.Request).Where(p => p.Id == id).FirstOrDefault();
			if(offer.Request.Status == "Completed")
			{
				return false;
			}
			if (offer.Status == "Accepted" && offer.Provider != null)
			{
				var timeSlot = _context.TimeSlots.Where(t => t.Id == offer.TimeSlotId).FirstOrDefault();

				DateTime offerTime = timeSlot.Date.ToDateTime(timeSlot.FromTime);
				DateTime TimeAfter24 = DateTime.Now.AddHours(24);
				TimeSpan timeDifference = TimeAfter24 - offerTime;

				// Check if the difference is greater than or equal to 24 hours
				if (offerTime <= TimeAfter24)
				{
					offer.Provider.Balance += (offer.Fees * 50) / 100;
				}
				offer.Request.Status = "Requested";
				timeSlot.ToTime = TimeOnly.MinValue;
				CreateAdminOffer(offer.Id);
			}
			_context.Remove(offer);
			return Save();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public ICollection<ServiceOffer> GetUnCompletedOffers(string providerId)
		{
			var offers = _context.Offers.Include(p => p.Provider).Include(p => p.Request).Where(p => p.Provider.Id == providerId).ToList(); ;

			if (offers != null)
			{
				var pendingOffers = offers.Where(o => o.Request.Status != "Completed").ToList();
				return pendingOffers;
			}

			return null;
		}
        public ICollection<ServiceOffer> GetOfffersOfProvider(string providerId)
		{
            var offers = _context.Offers.Include(p => p.Provider).Include(p => p.Request).Where(p => p.Provider.Id == providerId).ToList(); 
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

		public ICollection<GetCalendarDto> GetCalendarDetails(string ProviderId)
		{
			var offers = _context.Offers.Include(p => p.Provider).Include(p => p.Request).Where(p => p.Provider.Id == ProviderId).Where(p => p.Status == "Accepted").ToList();
			ICollection<GetCalendarDto> calendarDtos = new List<GetCalendarDto>();
			foreach (var offer in offers)
			{
				var acceptedTimeSlot = _context.TimeSlots.Where(p => p.Id == offer.TimeSlotId).FirstOrDefault();
				var request = _context.Requests.Include(p => p.Subcategory).Where(p => p.Id == offer.Request.Id).FirstOrDefault();
				var calendarDto = new GetCalendarDto
				{
					RequestId = offer.Request.Id,
					OfferId = offer.Id,
					Date = acceptedTimeSlot.Date.ToString("yyyy-M-d"),
					FromTime = acceptedTimeSlot.FromTime.ToString("HH:mm"),
					ToTime = acceptedTimeSlot.ToTime.ToString("HH:mm"),
					SubcategoryNameAr = request.Subcategory?.NameAr,
                    SubcategoryNameEn = request.Subcategory?.NameEn

                };
				calendarDtos.Add(calendarDto);
			}
			return calendarDtos;
		}
    }
}
