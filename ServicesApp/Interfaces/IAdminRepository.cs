using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IAdminRepository
	{
		ICollection<Admin> GetAdmins();
		Admin GetAdmin(string id);
		// Admin GetAdmin(string email, string password);
		bool AdminExist(string id);
	}
}
