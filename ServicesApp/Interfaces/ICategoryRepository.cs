using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICategoryRepository
	{
		ICollection<Category> GetCategories();
		Category GetCategory(int id);
		Category GetCategory(string name);
		bool CategoryExist(int id);
		bool CreateCategory(Category category);
		bool Save();
	}
}
