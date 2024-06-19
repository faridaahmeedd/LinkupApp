using Xunit;
using FakeItEasy;
using AutoMapper;
using FluentAssertions;
using System.Collections.Generic; 
using ServicesApp.Interfaces;
using ServicesApp.Dto.Category;
using ServicesApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Models;

namespace ServicesApp.Tests.Controller
{
	public class CategoryControllerTests
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public CategoryControllerTests()
        {
			_categoryRepository = A.Fake<ICategoryRepository>();
			_mapper = A.Fake<IMapper>();
		}

		[Fact]
		public void GetCategories_WhenCalled_ReturnsOk()
		{
			// Arrange
			var categories = A.Fake<ICollection<Category>>();
			var categoriesList = A.Fake<List<CategoryDto>>();
			A.CallTo(() => _categoryRepository.GetCategories()).Returns(categories);
			A.CallTo(() => _mapper.Map<List<CategoryDto>>(categories)).Returns(categoriesList);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.GetCategories();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetCategoryById_CategoryExists_ReturnsOk()
		{
			// Arrange
			var categoryId = 1;
			var category = A.Fake<Category>();
			var categoryDto = A.Fake<CategoryDto>();
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(true);
			A.CallTo(() => _categoryRepository.GetCategory(categoryId)).Returns(category);
			A.CallTo(() => _mapper.Map<CategoryDto>(category)).Returns(categoryDto);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.GetCategory(categoryId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetCategoryById_CategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var categoryId = 1;
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(false);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.GetCategory(categoryId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void CreateCategory_CategoryDoesNotExist_ReturnsOk()
		{
			// Arrange
			var categoryDto = A.Fake<CategoryDto>();
			var category = A.Fake<Category>();
			A.CallTo(() => _categoryRepository.GetCategory(categoryDto.NameEn)).Returns(null);
			A.CallTo(() => _categoryRepository.GetCategory(categoryDto.NameAr)).Returns(null);
			A.CallTo(() => _mapper.Map<Category>(categoryDto)).Returns(category);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.CreateCategory(categoryDto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void CreateCategory_CategoryAlreadyExists_ReturnsBadRequest()
		{
			// Arrange
			var categoryDto = A.Fake<CategoryDto>();
			var category = A.Fake<Category>();
			A.CallTo(() => _categoryRepository.GetCategory(categoryDto.NameEn)).Returns(category);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.CreateCategory(categoryDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public void UpdateCategory_CategoryExists_ReturnsOk()
		{
			// Arrange
			var categoryDto = A.Fake<CategoryDto>();
			var category = A.Fake<Category>();
			A.CallTo(() => _categoryRepository.CategoryExist(categoryDto.Id)).Returns(true);
			A.CallTo(() => _mapper.Map<Category>(categoryDto)).Returns(category);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.UpdateCategory(categoryDto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void UpdateCategory_CategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var categoryDto = A.Fake<CategoryDto>();
			A.CallTo(() => _categoryRepository.CategoryExist(categoryDto.Id)).Returns(false);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.UpdateCategory(categoryDto);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void DeleteCategory_CategoryExists_ReturnsOk()
		{
			// Arrange
			var categoryId = 1;
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(true);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.DeleteCategory(categoryId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void DeleteCategory_CategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var categoryId = 1;
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(false);
			var controller = new CategoryController(_categoryRepository, _mapper);

			// Act
			var result = controller.DeleteCategory(categoryId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}
	}
}