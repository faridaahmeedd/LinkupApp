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
        public DbSet<ServiceRequest> Requests { get; set; }
        public DbSet<ServiceOffer> Offers { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Provider> Providers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configure the relationship: One ServiceRequest has many TimeSlots
			modelBuilder.Entity<ServiceRequest>()
				.HasMany(sr => sr.TimeSlots)
				.WithOne(ts => ts.ServiceRequest)
				.HasForeignKey(ts => ts.ServiceRequestId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);     // SEARCHH

			// Configure the relationship: One ServiceRequest has many ServiceOffers
			modelBuilder.Entity<ServiceRequest>()
				.HasMany(sr => sr.Offers)
				.WithOne(so => so.Request)
				.HasForeignKey(so => so.ServiceRequestId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);     // SEARCHH
		}

	}
}
