using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface ICategoryRepository
	{
		ICollection<Category> GetCategories();

	}
}
