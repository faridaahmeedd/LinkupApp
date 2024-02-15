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

        public ICollection<ServiceRequest> GetServicesByCustomer(string customerId)
		{
			return _context.Requests.Where(p => p.Customer.Id == customerId).ToList();
		}

		public ICollection<ServiceRequest> GetUncompletedServices()
        {
			return _context.Requests.Where(p => p.Status == "Requested").ToList();
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
                if(existingService.Status == "Requested")
                {
					existingService.Description = updatedService.Description;
					existingService.Image = updatedService.Image;
					existingService.Location = updatedService.Location;
					return Save();
				}
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

        public ServiceOffer AcceptedOffer(int serviceId)
        {
            var serviceRequest = _context.Requests.FirstOrDefault(sr => sr.Id == serviceId);

            if (serviceRequest != null)
            {
                var acceptedOffer = _context.Offers.Include(o => o.Provider).FirstOrDefault(o => o.Request.Id == serviceId && o.Status == "Accepted");
				return acceptedOffer;
            }
            return null;
        }
      
        public ICollection<ServiceDetailsDto> GetAllServicesDetails()
        {
            var serviceDetails = _context.Requests
                .Include(r => r.Subcategory)
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
                    SubcategoryName = r.Subcategory.Name,
					MinFees = r.Subcategory.MinFees,
					MaxFees = r.Subcategory.MaxFees,
                    TimeSlots = r.TimeSlots.Select(t => new TimeSlotDto
                    {
                        Id = t.Id,
                        Date = t.Date.ToString(),
                        FromTime = t.FromTime.ToString()
                    }).ToList(),
                    Offers = r.Offers.Select(t => new ServiceOfferDto
                    {
                        Fees = t.Fees,
                        Duration = t.Duration.ToString(),
                        TimeSlotId = t.TimeSlotId
                    }).ToList()

                })
                .ToList();

            return serviceDetails;
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

		public ICollection<GetServiceRequestDto> ServiceDetailsForCustomer(string CustomerId)
		{
			Console.WriteLine("testt");
			Console.WriteLine(AcceptedOffer(11).Id);
			var requests = _context.Requests
				.Include(o => o.Customer)
				.Include(o => o.Offers)
				.ThenInclude(offer => offer.Provider)
				.Where(p => p.Customer.Id == CustomerId)
				.Select(o => new GetServiceRequestDto
				{
					Id = o.Id,
					Location = o.Location,
					Description = o.Description,
					Status = o.Status,
					Image = o.Image,
					ProviderName = o.Offers.FirstOrDefault().Provider.FName + " " + o.Offers.FirstOrDefault().Provider.LName,
					ProviderMobileNumber = o.Offers.FirstOrDefault().Provider.MobileNumber ?? " "
				})
				.ToList();

			Console.WriteLine(requests[0].Id);
			return requests;
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
