using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
    public class SubcategoryRepository : ISubcategoryRepository
	{
		private readonly DataContext _context;

        public SubcategoryRepository(DataContext context)
        {
            this._context = context;
        }
       
        public ICollection<Subcategory> GetSubcategories()
        {
            return _context.Subcategories.OrderBy(p => p.Id).ToList();
        }

		public ICollection<Subcategory> GetSubcategories(int categoryId)
		{
			return _context.Subcategories.Where(p => p.Category.Id == categoryId).ToList();
		}

		public Subcategory GetSubcategory(int subcategoryId)
		{
			return _context.Subcategories.Where(p => p.Id == subcategoryId).FirstOrDefault();
		}

		public Subcategory GetSubcategory(string name)
		{
			return _context.Subcategories.Where(p => p.NameEn.Trim().ToUpper() == name.ToUpper() || p.NameAr == name).FirstOrDefault();
		}

		public bool SubcategoryExist(int id)
		{
			return _context.Subcategories.Any(p => p.Id == id);
		}

        public bool SubcategoryExist(string name)
        {
            return _context.Subcategories.Any(p => p.NameEn == name || p.NameAr == name);
        }

        public bool CreateSubcategory(Subcategory subcategory)
		{
			_context.Add(subcategory);
			return Save();
		}

		public bool UpdateSubcategory(Subcategory subcategory)
		{
			_context.Update(subcategory);
			return Save();
		}

		public bool DeleteSubcategory(int id)
		{
            var subcategory = _context.Subcategories.Where(p => p.Id == id).FirstOrDefault();
            _context.Remove(subcategory);
			return Save();
        }

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
