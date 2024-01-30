using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Dto.Category;
using ServicesApp.Core.Models;
using AutoMapper;
using ServicesApp.APIs;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	[Authorize]
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
		[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
		public IActionResult GetCategories()
		{
			var category = _categoryRepository.GetCategories();
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(category);
		}

		[HttpGet("Id/{CategoryId:int}", Name ="GetCategoryById")]
		[ProducesResponseType(200, Type = typeof(Category))]
		public IActionResult GetCategory(int CategoryId)
		{
			if (!_categoryRepository.CategoryExist(CategoryId))
			{
				return NotFound(ApiResponse.CategoryNotFound);
			}
			var category = _categoryRepository.GetCategory(CategoryId);
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(category);
		}

		[HttpGet("{CategoryName:minlength(1)}")]
		[ProducesResponseType(200, Type = typeof(Category))]
		public IActionResult GetCategory(String CategoryName)
		{
			var category = _categoryRepository.GetCategory(CategoryName);
			if (category == null)
			{
				return NotFound(ApiResponse.CategoryNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(category);
		}

		[HttpPost]
		[Authorize(Roles = "Admin, MainAdmin")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
		{
			if (!ModelState.IsValid)
            {
				return BadRequest(ApiResponse.NotValid);
			}
			var category = _categoryRepository.GetCategory(categoryCreate.Name);
			if(category != null)
			{
				return BadRequest(ApiResponse.CategoryAlreadyExist);
			}
			
            var mapCategory = _mapper.Map<Category>(categoryCreate);
          
            if (!_categoryRepository.CreateCategory(mapCategory))
			{
				return StatusCode(500,ApiResponse.SomethingWrong);
			}
            categoryCreate.Id = mapCategory.Id;
			return Ok(new
			{
                CategoryId = categoryCreate.Id,
                statusMsg = "success",
                message = "Category Created Successfully."
            });
		}

		[HttpPut("update")]
		[Authorize(Roles = "Admin, MainAdmin")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult UpdateCategory([FromBody] CategoryDto categoryUpdate)
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

            if (!_categoryRepository.UpdateCategory(mapCategory))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessUpdated);
		}

		[HttpDelete("{CategoryId}")]
		[Authorize(Roles = "Admin, MainAdmin")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult DeleteCategory(int CategoryId)
		{
			if (!_categoryRepository.CategoryExist(CategoryId))
			{
				return NotFound(ApiResponse.CategoryNotFound);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}

			if (! _categoryRepository.DeleteCategory(CategoryId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}
	}
}