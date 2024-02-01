using ServicesApp.Dto;
using ServicesApp.Dto.Service;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceRequestRepository
	{
		ICollection<ServiceRequest> GetServices();
		ServiceRequest GetService(int id);
		ICollection<ServiceRequest> GetServicesByCustomer(string id);
		ICollection<ServiceRequest> GetUncompletedServices();
		bool ServiceExist(int id);
		ICollection<ServiceRequest> GetServicesWithFees();

        bool CreateService(ServiceRequest service);
		bool UpdateService(ServiceRequest service);
		bool DeleteService(int id);
		bool Save();
		//bool CheckServiceMinFees(ServiceRequest service, int categoryId);
        ICollection<ServiceOffer> GetOffersOfService(int id);
		ServiceOffer AcceptedOffer(int serviceId);
        bool TimeSlotsExistInService(int ServiceId, int timeSlotId);
		bool CompleteService(int id);
		ICollection<ServiceDetailsDto> GetAllServicesDetails();
		bool UpdateUnkownCategory(int serviceId, string categoryName);
		bool UpdateMaxFees(int serviceId, int maxFees);

    }
}
