using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
    public class AdminRepository : IAdminRepository
    {
        public readonly DataContext _context;
        public AdminRepository(DataContext context)
        {
            _context = context;
        }

        public bool AdminExist(string id)
        {
            return _context.Admins.Any(p => p.Id == id);
        }

        public Admin GetAdmin(string id)
        {
            //return _context.Admins.Where(p => p.Id == id).FirstOrDefault();
            return _context.Admins.FirstOrDefault(p => p.Id == id);
        }

        //public Admin GetAdmin(string email, string password)
        //{
        //    return _context.Admins.Where(p => p.Email == email && p. == password).FirstOrDefault();
        //}

        public ICollection<Admin> GetAdmins()
        {
            return _context.Admins.OrderBy(p => p.Id).ToList();
        }
        
        /*public async Task<ICollection<Admin>> GetAdmins()
        {
            return await _context.Admins.OrderBy(p => p.Id).ToListAsync();
        }*/

    }

   
}
