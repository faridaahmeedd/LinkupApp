using Xunit;
using FakeItEasy;
using AutoMapper;
using FluentAssertions;
using System.Collections.Generic;
using ServicesApp.Interfaces;
using ServicesApp.Dto.Subcategory;
using ServicesApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Models;

namespace ServicesApp.Tests.Controller
{
	public class SubcategoryControllerTests
	{
		private readonly ISubcategoryRepository _subcategoryRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public SubcategoryControllerTests()
		{
			_subcategoryRepository = A.Fake<ISubcategoryRepository>();
			_categoryRepository = A.Fake<ICategoryRepository>();
			_mapper = A.Fake<IMapper>();
		}

		[Fact]
		public void GetSubcategories_WhenCalled_ReturnsOk()
		{
			// Arrange
			var subcategories = A.Fake<ICollection<Subcategory>>();
			var subcategoriesList = A.Fake<List<SubcategoryDto>>();
			A.CallTo(() => _subcategoryRepository.GetSubcategories()).Returns(subcategories);
			A.CallTo(() => _mapper.Map<List<SubcategoryDto>>(subcategories)).Returns(subcategoriesList);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategories();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetSubcategoriesByCategoryId_CategoryExists_ReturnsOk()
		{
			// Arrange
			var categoryId = 1;
			var subcategories = A.Fake<ICollection<Subcategory>>();
			var mappedSubcategories = A.Fake<List<SubcategoryDto>>();
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(true);
			A.CallTo(() => _subcategoryRepository.GetSubcategories(categoryId)).Returns(subcategories);
			A.CallTo(() => _mapper.Map<List<SubcategoryDto>>(subcategories)).Returns(mappedSubcategories);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategories(categoryId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetSubcategoriesByCategoryId_CategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var categoryId = 1;
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(false);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategories(categoryId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetSubcategoryById_SubcategoryExists_ReturnsOk()
		{
			// Arrange
			var subcategoryId = 1;
			var subcategory = A.Fake<Subcategory>();
			var subcategoryDto = A.Fake<SubcategoryDto>();
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryId)).Returns(true);
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryId)).Returns(subcategory);
			A.CallTo(() => _mapper.Map<SubcategoryDto>(subcategory)).Returns(subcategoryDto);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategory(subcategoryId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetSubcategoryById_SubcategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var subcategoryId = 1;
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryId)).Returns(false);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategory(subcategoryId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetSubcategoryByName_SubcategoryExists_ReturnsOk()
		{
			// Arrange
			var subcategoryName = "Test Subcategory";
			var subcategory = A.Fake<Subcategory>();
			var subcategoryDto = A.Fake<SubcategoryDto>();
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryName)).Returns(subcategory);
			A.CallTo(() => _mapper.Map<SubcategoryDto>(subcategory)).Returns(subcategoryDto);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategory(subcategoryName);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetSubcategoryByName_SubcategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var subcategoryName = "Nonexistent Subcategory";
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryName)).Returns(null);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.GetSubcategory(subcategoryName);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void CreateSubcategory_SubcategoryDoesNotExist_ReturnsOk()
		{
			// Arrange
			var categoryId = 1;
			var subcategoryDto = A.Fake<SubcategoryDto>();
			var subcategory = A.Fake<Subcategory>();
			A.CallTo(() => _categoryRepository.CategoryExist(categoryId)).Returns(true);
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryDto.NameEn)).Returns(null);
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryDto.NameAr)).Returns(null);
			A.CallTo(() => _mapper.Map<Subcategory>(subcategoryDto)).Returns(subcategory);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.CreateSubcategory(categoryId, subcategoryDto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void CreateSubcategory_SubcategoryAlreadyExists_ReturnsBadRequest()
		{
			// Arrange
			var categoryId = 1;
			var subcategoryDto = A.Fake<SubcategoryDto>();
			var subcategory = A.Fake<Subcategory>();
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryDto.NameEn)).Returns(subcategory);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.CreateSubcategory(categoryId, subcategoryDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public void UpdateSubcategory_SubcategoryExists_ReturnsOk()
		{
			// Arrange
			var subcategoryDto = A.Fake<SubcategoryDto>();
			var subcategory = A.Fake<Subcategory>();
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryDto.Id)).Returns(true);
			A.CallTo(() => _mapper.Map<Subcategory>(subcategoryDto)).Returns(subcategory);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.UpdateSubcategory(subcategoryDto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void UpdateSubcategory_SubcategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var subcategoryDto = A.Fake<SubcategoryDto>();
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryDto.Id)).Returns(false);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.UpdateSubcategory(subcategoryDto);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void DeleteSubcategory_SubcategoryExists_ReturnsOk()
		{
			// Arrange
			var subcategoryId = 1;
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryId)).Returns(true);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.DeleteSubcategory(subcategoryId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void DeleteSubcategory_SubcategoryDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var subcategoryId = 1;
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryId)).Returns(false);
			var controller = new SubcategoryController(_subcategoryRepository, _categoryRepository, _mapper);

			// Act
			var result = controller.DeleteSubcategory(subcategoryId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}
	}
}
