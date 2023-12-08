using ServicesApp.Data;
using ServicesApp.Dto;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
	public class ServiceRepository : IServiceRepository
	{
		private readonly DataContext _context;

		public ServiceRepository(DataContext context)
		{
			this._context = context;
		}

		public ICollection<Service> GetServices()
		{
			return _context.Services.OrderBy(p => p.Id).ToList();
		}

		public Service GetService(int id)
		{
			return _context.Services.Where(p => p.Id == id).FirstOrDefault();
		}

		public Service GetService(string name)
		{
			return _context.Services.Where(p => p.Name == name).FirstOrDefault();
		}
		public bool ServiceExist(int id)
		{
			return _context.Services.Any(p => p.Id == id);
		}

		public bool CreateService(Service service)
		{
			// Change Tracker (add,update,modify)
			_context.Add(service);
			return Save();
		}

		public bool UpdateService(Service service)
		{
			// Change Tracker (add,update,modify)
			_context.Update(service);
			return Save();
		}

		public bool DeleteService(int id)
		{
			var service = _context.Services.Where(p => p.Id == id).FirstOrDefault();
			_context.Remove(service!);
			return Save();
		}

		public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

	}
}
