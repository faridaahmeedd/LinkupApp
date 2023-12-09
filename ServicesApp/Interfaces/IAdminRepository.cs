using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IAdminRepository
	{
		ICollection<Admin> GetAdmins();
		Admin GetAdmin(int id);
		Admin GetAdmin(string email, string password);
		bool AdminExist(int id);
	}
}
