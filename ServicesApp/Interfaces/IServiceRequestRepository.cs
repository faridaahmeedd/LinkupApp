using ServicesApp.Dto;
using ServicesApp.Dto.Service;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceRequestRepository
	{
		ICollection<ServiceRequest> GetServices();
		ServiceRequest GetService(int id);
		ICollection<ServiceRequest> GetServicesByCustomer(string customerId);
		ICollection<ServiceRequest> GetUncompletedServices();
		bool ServiceExist(int id);
		bool CreateService(ServiceRequest service);
		bool UpdateService(ServiceRequest service);
		bool DeleteService(int id);
		bool Save();
        ICollection<ServiceOffer> GetUndeclinedOffersOfService(int id);
		ServiceOffer GetAcceptedOffer(int serviceId);
        bool TimeSlotsExistInService(int ServiceId, int timeSlotId);
		bool CompleteService(int id);
		bool UpdateUnknownSubcategory(int serviceId, string subcategoryName);
		//bool CheckServiceMinFees(ServiceRequest service, int categoryId);
		//bool UpdateMaxFees(int serviceId, int maxFees);
		//ICollection<ServiceRequest> GetServicesWithFees();
		//ICollection<ServiceRequest> GetServicesWithFees(string customerId);
	}
}
