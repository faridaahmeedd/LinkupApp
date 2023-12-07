using ServicesApp.Core.Models;
using ServicesApp.Data;
using ServicesApp.Models;

namespace PokemonReviewApp
{
	public class Seed(DataContext context)
	{
		private readonly DataContext dataContext = context;

		public void SeedDataContext()
		{
			if (!dataContext.Customers.Any())
			{
				var Customers = new List<Customer>()
				{
					new Customer()
					{
						services = new List<Service>()
						{
							new Service()
							{
								Category = new Category()
								{
									Id = 1,
									Name = "Category1",
									Description = "Description1",
									minFees = 1,
								},
								CategoryId = 1,
								CustomerId = 1,
								Description = "Description1",
								Id = 1,
								Fees = 1,
								Name = "Service1",
							}
						},
						City = "Giza",
						Country = "Egypt",
						Email = "fefe@gmail.com",
						FName = "fefe",
						LName = "ahmed",
						Gender = true,
						Id = 1,
						Password = "Password",
						PhoneNumber = "1234567890",
						BirthDate = DateOnly.MaxValue,
					},
				};
				dataContext.Customers.AddRange(Customers);
				dataContext.SaveChanges();
			}
		}
	}
}