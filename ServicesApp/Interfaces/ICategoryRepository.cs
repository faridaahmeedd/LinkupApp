using ServicesApp.Dto.Category;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICategoryRepository
	{
		ICollection<Category> GetCategories();
		Category GetCategory(int id);
		Category GetCategory(string name);
		bool CategoryExist(int id);
		bool CategoryExist(string name);
        bool CreateCategory(Category category);
		bool UpdateCategory(Category category);
		bool DeleteCategory(int id);
		bool Save();
	}
}
