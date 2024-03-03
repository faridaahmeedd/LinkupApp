using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Models;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection.Emit;

namespace ServicesApp.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
		public DbSet<Subcategory> Subcategories { get; set; }
		public DbSet<ServiceRequest> Requests { get; set; }
        public DbSet<ServiceOffer> Offers { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Provider> Providers { get; set; }
		public DbSet<Review> Reviews { get; set; }
		//public DbSet<Admin> Admins { get; set; }

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
            modelBuilder
                .Entity<Subcategory>()
                .HasMany(p => p.Services)
                .WithOne(o => o.Subcategory)
                .OnDelete(DeleteBehavior.SetNull);
            base.OnModelCreating(modelBuilder);
		    SeedRoles(modelBuilder);
			SeedMainAdminUser(modelBuilder);
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
					new IdentityRole() { Id = "6e83945a-31f7-4a85-9679-e5e12895df12", Name = "Customer", ConcurrencyStamp = "1", NormalizedName = "Customer" },
					new IdentityRole() { Id = "43626702-ab6b-4481-89f0-769da1a485c2", Name = "Provider", ConcurrencyStamp = "2", NormalizedName = "Provider" },
					new IdentityRole() { Id = "fee70a81-e665-4566-afc0-5d0c84e3f4fe", Name = "Admin", ConcurrencyStamp = "3", NormalizedName = "Admin" },
					new IdentityRole() { Id = "5fe9bbcd-eb74-448e-9580-1c4bd31f7958", Name = "MainAdmin", ConcurrencyStamp = "4", NormalizedName = "MainAdmin" }
				);
		}

		private void SeedMainAdminUser(ModelBuilder builder)
		{
			var hasher = new PasswordHasher<AppUser>();
			builder.Entity<AppUser>().HasData(
			   new AppUser()
			   {
				   Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
				   UserName = "MainAdmin",
				   Email = "MainAdmin@gmail.com",
				   NormalizedUserName = "MainAdmin".ToUpper(),
				   NormalizedEmail = "MainAdmin@gmail.com".ToUpper(),
				   PasswordHash = hasher.HashPassword(null, "Admin123-"),
				   EmailConfirmed = true,
				   LockoutEnabled = true,
				   PhoneNumberConfirmed = true,
				   SecurityStamp = Guid.NewGuid().ToString()
			   }
			);
			//Seeding the relation between our user and role to AspNetUserRoles table

			builder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string>()
				{
					RoleId = "5fe9bbcd-eb74-448e-9580-1c4bd31f7958", 
					UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
				}
			);
		}
	}
}
