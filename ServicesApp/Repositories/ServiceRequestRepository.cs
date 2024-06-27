using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System.Collections.Generic;

namespace ServicesApp.Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
	{
		private readonly DataContext _context;
		private readonly ITimeSlotsRepository _timeSlotsRepository;
		private readonly IMLRepository _MLRepository;

		public ServiceRequestRepository(DataContext context, ITimeSlotsRepository timeSlotsRepository, IMLRepository MLRepository)
		{
			_context = context;
			_timeSlotsRepository = timeSlotsRepository;
			_MLRepository = MLRepository;
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

		public int CreateRequestAfterExamination(int ServiceId)
		{
            var existingService = GetService(ServiceId);

            if (existingService != null)
			{
				var newService = new ServiceRequest
				{
					Description = existingService.ExaminationComment,
					Image = existingService.Image,
					Location = existingService.Location,
					PaymentMethod = existingService.PaymentMethod,
					PaymentStatus = "Pending",
					Status = "Requested",
					Customer = existingService.Customer,
					Subcategory = existingService.Subcategory,
					Volunteer = existingService.Volunteer,
				};
				_context.Requests.Add(newService);
                Save();
                return newService.Id;
			}
			return 0;
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
					existingService.PaymentMethod = updatedService.PaymentMethod;
				}
				return Save();
			}
            return false;
        }

		public bool AddExaminationComment(int ServiceId, string Comment)
		{
			var existingService = _context.Requests.Find(ServiceId);
			
			if (existingService != null)
			{
				existingService.ExaminationComment = Comment;
				return Save();
			}
			return false;
		}

		public bool DeleteService(int id)
		{
			var service = _context.Requests.Include(c => c.Customer).Where(p => p.Id == id).FirstOrDefault();
			if (service.Status == "Completed")
			{
				return false;
			}
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

		public ICollection<ServiceOffer> GetUndeclinedOffersOfService(int id)
		{
            var request = _context.Requests.Include(o => o.Offers).FirstOrDefault(o => o.Id == id);
           
            if (request != null)
            {
				var Offers = _context.Offers.Include(o => o.Provider).Where(o => o.Request.Id == id && o.Status != "Declined").ToList();
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
                if (service.Subcategory.NameEn == "Unknown")
                {
                    var subcategory = _context.Subcategories.Where(c => c.NameEn == subcategoryName).FirstOrDefault();
                    service.Subcategory = subcategory;
                    return Save();
                }
                if (service.Subcategory.NameAr == "غير محدد")
                {
                    var subcategory = _context.Subcategories.Where(c => c.NameAr == subcategoryName).FirstOrDefault();
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

		public ICollection<GetCalendarDto> GetCalendarDetails(string CustomerId)
		{
			var requests = _context.Requests.Include(p => p.Customer).Include(p => p.Subcategory).Where(p => p.Customer.Id == CustomerId).Where(p => p.Status != "Requested").ToList();
			ICollection<GetCalendarDto> calendarDtos = new List<GetCalendarDto>();
			foreach(var request in requests)
			{
				var acceptedTimeSlot = _timeSlotsRepository.GetAcceptedTimeSlot(request.Id);
				var calendarDto = new GetCalendarDto
				{
					RequestId = request.Id,
					OfferId = (GetAcceptedOffer(request.Id)).Id,
					Date = acceptedTimeSlot.Date.ToString("yyyy-M-d"),
					FromTime = acceptedTimeSlot.FromTime.ToString("HH:mm"),
					ToTime = acceptedTimeSlot.ToTime.ToString("HH:mm"),
					SubcategoryNameEn = request.Subcategory?.NameEn,
                    SubcategoryNameAr = request.Subcategory?.NameAr

                };
				calendarDtos.Add(calendarDto);
			}
			return calendarDtos;
		}

		public bool CheckRequestCompleted(int requestId)
		{
			var request = _context.Requests.FirstOrDefault(r => r.Id == requestId && r.Status == "Completed");
			if (request != null)
			{
				return true;
			}
			return false;
		}

		//public ICollection<ServiceRequest> GetMatchedRequestsOfProvider(string providerId)
		//{
		//	var requests = GetUncompletedServices();
		//	var matchedRequests = new List<ServiceRequest>();

		//	foreach (var request in requests)
		//	{
		//		bool isMatched = _MLRepository.MatchJobAndService(request.Id, providerId).Result;
		//		if (isMatched)
		//		{
		//			matchedRequests.Add(request);
		//		}
		//	}
		//	return matchedRequests;
		//}
	}
}
