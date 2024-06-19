using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Models;
using ServicesApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Tests.Repository
{
	public class CategoryRepositoryTests
	{
		private async Task<DataContext> GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new DataContext(options);
			databaseContext.Database.EnsureCreated();
			if (await databaseContext.Categories.CountAsync() <= 0)
			{
				for (int i = 1; i <= 10; i++)
				{
					databaseContext.Categories.Add(
					new Category()
					{
						NameEn = "Home",
						DescriptionEn = "Home Services",
						NameAr = "البيت",
						DescriptionAr = "خدمات البيت"
					});
					await databaseContext.SaveChangesAsync();
				}
			}
			return databaseContext;
		}

		[Fact]
		public async void GetCategoryByName_CategoryFound_ReturnsCategory()
		{
			// Arrange
			var categoryName = "Home";
			var dbContext = await GetDatabaseContext();
			var categoryRepository = new CategoryRepository(dbContext);

			// Act
			var result = categoryRepository.GetCategory(categoryName);

			// Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<Category>();
		}

		[Fact]
		public async void GetCategoryByName_CategoryNotFound_ReturnsNull()
		{
			// Arrange
			var categoryName = "NonExistentCategory";
			var dbContext = await GetDatabaseContext();
			var categoryRepository = new CategoryRepository(dbContext);

			// Act
			var result = categoryRepository.GetCategory(categoryName);

			// Assert
			result.Should().BeNull();
		}
	}
}
