using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IAdminRepository
	{
		Task<bool> AdminExist(string id);
		Task<AppUser> GetAdmin(string id);
		Task<ICollection<AppUser>> GetAdmins();
		bool CreateAdmin(Admin admin);
		Task<bool> DeleteAdmin(string id);
	}
}
