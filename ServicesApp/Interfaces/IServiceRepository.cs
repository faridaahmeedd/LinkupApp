using ServicesApp.Dto;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceRepository
	{
		ICollection<Service> GetServices();
		Service GetService(int id);
		Service GetService(string name);
		bool ServiceExist(int id);
		bool CreateService(Service service);
		bool Save();
	}
}
