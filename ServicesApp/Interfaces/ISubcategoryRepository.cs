using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ISubcategoryRepository
	{
		ICollection<Subcategory> GetSubcategories();
		ICollection<Subcategory> GetSubcategories(int categoryId);
		Subcategory GetSubcategory(int id);
		Subcategory GetSubcategory(string name);
		bool SubcategoryExist(int id);
		bool SubcategoryExist(string name);
        bool CreateSubcategory(Subcategory subcategory);
		bool UpdateSubcategory(Subcategory subcategory);
		bool DeleteSubcategory(int id);
		bool Save();
	}
}
