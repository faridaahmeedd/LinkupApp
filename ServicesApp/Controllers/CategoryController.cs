using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Dto.Category;
using AutoMapper;
using ServicesApp.Helper;

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
					return BadRequest(ApiResponses.NotValid);
				}
				var categories = _categoryRepository.GetCategories();
				var mapCategories = _mapper.Map<List<CategoryDto>>(categories);
				return Ok(mapCategories);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{CategoryId}")]
		public IActionResult GetCategory(int CategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponses.CategoryNotFound);
				}
				var category = _categoryRepository.GetCategory(CategoryId);
				var mapCategory = _mapper.Map<CategoryDto>(category);
				return Ok(mapCategory);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{CategoryName:minlength(3)}")]
		public IActionResult GetCategory(String CategoryName)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var category = _categoryRepository.GetCategory(CategoryName);
				if (category == null)
				{
					return NotFound(ApiResponses.CategoryNotFound);
				}
				var mapCategory = _mapper.Map<CategoryDto>(category);
				return Ok(mapCategory);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPost]
		public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var categoryAr = _categoryRepository.GetCategory(categoryCreate.NameAr);
                var categoryEn = _categoryRepository.GetCategory(categoryCreate.NameEn);

                if (categoryAr != null || categoryEn !=null)
				{
					return BadRequest(ApiResponses.CategoryAlreadyExist);
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
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut()]
		public IActionResult UpdateCategory([FromBody] CategoryDto categoryUpdate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_categoryRepository.CategoryExist(categoryUpdate.Id))
				{
					return NotFound(ApiResponses.CategoryNotFound);
				}
				var mapCategory = _mapper.Map<Category>(categoryUpdate);

				_categoryRepository.UpdateCategory(mapCategory);
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpDelete("{CategoryId}")]
		public IActionResult DeleteCategory(int CategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponses.CategoryNotFound);
				}
				_categoryRepository.DeleteCategory(CategoryId);
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
	}
}