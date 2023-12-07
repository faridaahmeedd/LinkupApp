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

    }
}
