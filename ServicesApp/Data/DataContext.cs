using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Models;

namespace ServicesApp.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServiceRequest> Requests { get; set; }
        public DbSet<ServiceOffer> Offers { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Customer> Customers { get; set; }
        //public DbSet<Admin> Admins { get; set; }
        public DbSet<Provider> Providers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

            base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<AppUser>().Ignore(c => c.PhoneNumber)
											   .Ignore(c => c.PhoneNumberConfirmed);
            modelBuilder
                .Entity<Customer>()
                .HasMany(c => c.Services)
                .WithOne(s => s.Customer)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder
                .Entity<Provider>() 
                .HasMany(p => p.Offers)
                .WithOne(o => o.Provider)
                .OnDelete(DeleteBehavior.SetNull);
            base.OnModelCreating(modelBuilder);
		    SeedRoles(modelBuilder);
			//modelBuilder.HasDefaultSchema("dbo");
			//modelBuilder.Entity<IdentityUser>().ToTable("Users");
			//modelBuilder.Entity<IdentityRole>().ToTable("Roles");
			//modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
			//modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
			//modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
			//modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
			//modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
		}

		// Adding User Roles to DB
		private void SeedRoles(ModelBuilder builder)
		{
			builder.Entity<IdentityRole>().HasData(
					new IdentityRole() { Name = "Customer", ConcurrencyStamp = "1", NormalizedName = "Customer" },
					new IdentityRole() { Name = "Provider", ConcurrencyStamp = "2", NormalizedName = "Provider" },
					new IdentityRole() { Name = "Admin", ConcurrencyStamp = "3", NormalizedName = "Admin" }
				);
		}
	}
}
