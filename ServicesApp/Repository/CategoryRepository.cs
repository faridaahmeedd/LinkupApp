using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            this._context = context;
        }
       
        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(p => p.Id).ToList();
        }

		public Category GetCategory(int id)
		{
			return _context.Categories.Where(p => p.Id == id).FirstOrDefault();
		}

		public Category GetCategory(string name)
		{
			return _context.Categories.Where(p => p.Name == name).FirstOrDefault();
		}
		public bool CategoryExist(int id)
		{
			return _context.Categories.Any(p => p.Id == id);
		}

		public bool CreateCategory(Category category)
		{
			// Change Tracker (add,update,modify)
			_context.Add(category);
			return Save();

		}

		public bool Save()
		{
			//sql code is generated here
			var saved = _context.SaveChanges();
			return saved>0 ? true : false;
		}
	}
}
