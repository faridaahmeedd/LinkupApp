using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Data;
using ServicesApp.Dto;
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
                existingService.Fees = updatedService.Fees;
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
			var service = _context.Requests.Where(p => p.Id == id).FirstOrDefault();
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
	}
}
