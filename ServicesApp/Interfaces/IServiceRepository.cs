using ServicesApp.Dto;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceRepository
	{
		ICollection<ServiceRequest> GetServices();
		ServiceRequest GetService(int id);
		bool ServiceExist(int id);
		bool CreateService(ServiceRequest service);
		bool UpdateService(ServiceRequest service);
		bool DeleteService(int id);
		bool Save();
	}
}
