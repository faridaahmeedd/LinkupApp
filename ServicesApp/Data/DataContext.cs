using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) 
		{

		}
		public DbSet<Category> Categories { get; set; }
		public DbSet<Service> Services { get; set; }
		public DbSet<Customer> Customers { get; set; }
	}
}
