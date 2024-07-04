using Microsoft.AspNetCore.Identity;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;
		private readonly UserManager<AppUser> _userManager;

		public AdminRepository(DataContext context, UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
		}

        public async Task<bool> AdminExist(string id)
        {
			var admin = await _userManager.FindByIdAsync(id);
			if(admin == null)
			{
				return false;
			}
			var result = await _userManager.IsInRoleAsync(admin, "Admin");
			return result;
        }

		public async Task<AppUser> GetAdmin(string id)
        {
			var admin = await _userManager.FindByIdAsync(id);
			if(admin != null)
			{
				if(await _userManager.IsInRoleAsync(admin, "Admin"))
					return admin;
			}
			return null;
        }

        public async Task<ICollection<AppUser>> GetAdmins()
        {
			var admins = await _userManager.GetUsersInRoleAsync("Admin");
			return admins;
        }

		public async Task<bool> DeleteAdmin(string id)
		{
			var admin = await GetAdmin(id);
			if(admin != null)
			{
				await _userManager.DeleteAsync(admin);
				return true;
			}
			return false;
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
