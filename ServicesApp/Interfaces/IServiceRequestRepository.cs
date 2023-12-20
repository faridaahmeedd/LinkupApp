using ServicesApp.Dto;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceRequestRepository
	{
		ICollection<ServiceRequest> GetServices();
		ServiceRequest GetService(int id);
		bool ServiceExist(int id);
		bool CreateService(ServiceRequest service);
		bool UpdateService(ServiceRequest service);
		bool DeleteService(int id);
		bool Save();
		public bool TimeSlotsExistInService(int ServiceId, int timeSlotId);
	}
}
