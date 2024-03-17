using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
	{
		private readonly DataContext _context;

		public ServiceRequestRepository(DataContext context)
		{
			_context = context;
		}

		public ICollection<ServiceRequest> GetServices()
		{
			return _context.Requests.Include(p => p.Subcategory).Include(p => p.Customer).OrderBy(p => p.Id).ToList();
		}

		public ServiceRequest GetService(int id)
		{
			return _context.Requests.Include(p => p.Subcategory).Include(p => p.Customer).Where(p => p.Id == id).FirstOrDefault();
		}

        public ICollection<ServiceRequest> GetServicesByCustomer(string customerId)
		{
			return _context.Requests.Include(p => p.Subcategory).Include(p => p.Customer).Where(p => p.Customer.Id == customerId).ToList();
		}

		public ICollection<ServiceRequest> GetUncompletedServices()
        {
			return _context.Requests.Include(p => p.Subcategory).Include(p => p.Customer).Where(p => p.Status == "Requested").ToList();
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
            if (existingService != null)
            {
				existingService.PaymentStatus = updatedService.PaymentStatus;
				if (existingService.Status == "Requested")
                {
					existingService.Description = updatedService.Description;
					existingService.Image = updatedService.Image;
					existingService.Location = updatedService.Location;
				}
				return Save();
			}
            return false;
        }

        public bool DeleteService(int id)
		{
			var service = _context.Requests.Include(c => c.Customer).Where(p => p.Id == id).FirstOrDefault();
            if(service.Status == "Pending")
            {
				var offer = _context.Offers.Include(c => c.Request).Where(p => p.Request.Id == id && p.Status == "Accepted").FirstOrDefault();
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

		public bool TimeSlotsExistInService(int ServiceId, int timeSlotId)
		{
			var timeSlots = _context.TimeSlots.Where(p => p.ServiceRequest.Id == ServiceId);
			return timeSlots.Any(p => p.Id == timeSlotId);
		}

        public bool CompleteService(int id)
        {
			var request = _context.Requests.Include(o => o.Offers).FirstOrDefault(o => o.Id == id);
            if (request != null)
            {
                request.Status = "Completed";
				if(request.Offers != null)
				{
					request.Offers = request.Offers.Where(item => item.Status == "Accepted").ToList();
				}
                return Save();
            }
            return false;
        }

		public ICollection<ServiceOffer> GetUndeclinedOffersOfService(int id )
		{
            var request = _context.Requests.Include(o => o.Offers).FirstOrDefault(o => o.Id == id);
           
            if (request != null)
            {
                var Offers = request.Offers.Where(o => o.Status != "Declined").ToList();
                if (request.Offers != null)
                {
                    return Offers;
                }
            }
            return null;
        }

        public ServiceOffer GetAcceptedOffer(int serviceId)
        {
            var serviceRequest = _context.Requests.FirstOrDefault(sr => sr.Id == serviceId);

            if (serviceRequest != null)
            {
                var acceptedOffer = _context.Offers.Include(o => o.Provider).FirstOrDefault(o => o.Request.Id == serviceId && o.Status == "Accepted");
				return acceptedOffer;
            }
            return null;
        }
      
        public bool UpdateUnknownSubcategory(int serviceId, string subcategoryName)
        {
            var service = _context.Requests.Include(c => c.Subcategory).Where(s => s.Id == serviceId).FirstOrDefault();
            if (service != null)
            {
                if (service.Subcategory.Name == "Unknown")
                {
                    var subcategory = _context.Subcategories.Where(c => c.Name == subcategoryName).FirstOrDefault();
                    service.Subcategory = subcategory;
                    return Save();
                }
            }
            return false;
        }

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		
		//public ICollection<ServiceRequest> GetServicesWithFees()
		//{
		//    return _context.Requests.Where(p => p.MaxFees != 0).ToList();
		//}

		//public ICollection<ServiceRequest> GetServicesWithFees(string customerId)
		//{
		//	return _context.Requests.Where(p => p.Customer.Id == customerId && p.MaxFees != 0).ToList();
		//}

		//public bool UpdateMaxFees(int serviceId, int maxFees)
		//{
		//	var service = _context.Requests.Where(s => s.Id == serviceId).FirstOrDefault();
		//	if (service != null)
		//	{
		//		service.MaxFees = maxFees;
		//		return Save();
		//	}
		//	return false;
		//}
	}
}
