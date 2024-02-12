using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Dto.Category;
using ServicesApp.Core.Models;
using AutoMapper;
using ServicesApp.APIs;
using ServicesApp.Dto.Subcategory;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository , IMapper mapper)
        {
            _categoryRepository = categoryRepository;
			_mapper = mapper;
        }

		[HttpGet]
		public IActionResult GetCategories()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var categories = _categoryRepository.GetCategories();
				var mapCategories = _mapper.Map<List<CategoryDto>>(categories);
				return Ok(mapCategories);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{CategoryId:int}", Name ="GetCategoryById")]
		public IActionResult GetCategory(int CategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponse.CategoryNotFound);
				}
				var category = _categoryRepository.GetCategory(CategoryId);
				var mapCategory = _mapper.Map<CategoryDto>(category);
				return Ok(mapCategory);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{CategoryName:minlength(3)}")]
		public IActionResult GetCategory(String CategoryName)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var category = _categoryRepository.GetCategory(CategoryName);
				if (category == null)
				{
					return NotFound(ApiResponse.CategoryNotFound);
				}
				var mapCategory = _mapper.Map<CategoryDto>(category);
				return Ok(mapCategory);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPost]
		public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var category = _categoryRepository.GetCategory(categoryCreate.Name);
				if (category != null)
				{
					return BadRequest(ApiResponse.CategoryAlreadyExist);
				}
				var mapCategory = _mapper.Map<Category>(categoryCreate);

				_categoryRepository.CreateCategory(mapCategory);
				return Ok(new
				{
					statusMsg = "success",
					message = "Category Created Successfully.",
					CategoryId = mapCategory.Id,
				});
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut()]
		public IActionResult UpdateCategory([FromBody] CategoryDto categoryUpdate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_categoryRepository.CategoryExist(categoryUpdate.Id))
				{
					return NotFound(ApiResponse.CategoryNotFound);
				}
				var mapCategory = _mapper.Map<Category>(categoryUpdate);

				_categoryRepository.UpdateCategory(mapCategory);
				return Ok(ApiResponse.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpDelete("{CategoryId}")]
		public IActionResult DeleteCategory(int CategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponse.CategoryNotFound);
				}
				_categoryRepository.DeleteCategory(CategoryId);
				return Ok(ApiResponse.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
	}
}