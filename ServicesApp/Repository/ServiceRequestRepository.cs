using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Data;
using ServicesApp.Dto;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServicesApp.Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
	{
		private readonly DataContext _context;

		public ServiceRequestRepository(DataContext context)
		{
			this._context = context;
		}

		public ICollection<ServiceRequest> GetServices()
		{
			return _context.Requests.OrderBy(p => p.Id).ToList();
		}

		public ServiceRequest GetService(int id)
		{
			return _context.Requests.Where(p => p.Id == id).FirstOrDefault();
		}

		public bool ServiceExist(int id)
		{
			return _context.Requests.Any(p => p.Id == id);
		}

		public  bool CreateService(ServiceRequest service)
		{
			_context.Add(service);
			return Save();
		}

        public bool UpdateService(ServiceRequest updatedService)
        {
            var existingService = _context.Requests.Find(updatedService.Id);
            Console.WriteLine(existingService);
            Console.WriteLine("before");
            if (existingService != null)
            {
                existingService.Description = updatedService.Description;
                existingService.Image = updatedService.Image; //TODO

                _context.SaveChanges();
                Console.WriteLine("After");

                return true;
            }
            Console.WriteLine("error");

            return false;
        }

        public bool DeleteService(int id)
		{
			var service = _context.Requests.Include(c => c.Customer).Where(p => p.Id == id).FirstOrDefault();
            if(service.Status == "Pending")
            {
				var offer = _context.Offers.Include(c => c.Request).Where(p => p.Request.Id == id && p.Accepted == true).FirstOrDefault();
                var timeSlot = _context.TimeSlots.Where(t => t.Id == offer.TimeSlotId).FirstOrDefault();

				DateTime offerTime = timeSlot.Date.ToDateTime(timeSlot.FromTime);
				DateTime TimeAfter24 = DateTime.Now.AddHours(24); 
				TimeSpan timeDifference = TimeAfter24 - offerTime;    

				// Check if the difference is greater than or equal to 24 hours
				if (offerTime <= TimeAfter24)
                {
					service.Customer.Balance += (offer.Fees * 10)/100 ;
				}
            }
			_context.Remove(service!);
			return Save();
		}

		public bool Save()
		{
            //sql code is generated here
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

		public bool TimeSlotsExistInService(int ServiceId, int timeSlotId)
		{
			var timeSlots = _context.TimeSlots.Where(p => p.ServiceRequest.Id == ServiceId);
			return timeSlots.Any(p => p.Id == timeSlotId);
		}

        public bool CompleteService(int id)
        {
			var request = _context.Requests.Include(o => o.Offers).FirstOrDefault(o => o.Id == id);
			//var request = _context.Requests.Find(id);
            if (request != null)
            {
                request.Status = "Completed";
				if(request.Offers != null)
				{
					request.Offers = request.Offers.Where(item => item.Accepted == true).ToList();
				}
                _context.SaveChanges();
                return true;
            }
            return false;
        }
		public ICollection<ServiceOffer> GetOffersOfService(int id )
		{
            var request = _context.Requests.Include(o => o.Offers).FirstOrDefault(o => o.Id == id);
           
            if (request != null)
            {
                var Offers = request.Offers;
                if (request.Offers != null)
                {
                    return Offers;
                }
            }
            return null;
        }
        public ServiceOffer AcceptedOffer(int serviceId)
        {
            var serviceRequest = _context.Requests.Include(sr => sr.Offers).FirstOrDefault(sr => sr.Id == serviceId);

            if (serviceRequest != null)
            {
                var acceptedOffer = serviceRequest.Offers.FirstOrDefault(o => o.Accepted);
                return acceptedOffer;
            }

            return null;
        }

        public ICollection<ServiceDetailsDto> GetAllServicesDetails()
        {
            var serviceDetails = _context.Requests
                .Include(r => r.Category)
                .Include(r => r.Customer)
                 .Include(r => r.Offers)
                .Include(r => r.TimeSlots)
                .Select(r => new ServiceDetailsDto
                {
                    Id = r.Id,
                    Description = r.Description,
                    Status = r.Status,
                    CustomerName = r.Customer.FName,
                    CustomerId = r.Customer.Id,
                    CategoryName = r.Category.Name,
                    TimeSlots = r.TimeSlots.Select(t => new TimeSlotDto
                    {
                        Id = t.Id,
                        Date = t.Date,
                        FromTime = t.FromTime
                    }).ToList(),
                    Offers = r.Offers.Select(t => new ServiceOfferDto
                    {
                        Fees = t.Fees,
                        Duration = t.Duration,
                        TimeSlotId = t.TimeSlotId,
                        ProviderId = t.Provider.Id,  //
                        RequestId = t.Request.Id     //
                    }).ToList()

                })
                .ToList();

            return serviceDetails;
        }
    }
}
