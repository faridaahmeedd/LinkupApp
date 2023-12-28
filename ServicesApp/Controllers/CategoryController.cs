using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Dto.Category;
using ServicesApp.Core.Models;
using AutoMapper;

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
		[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
		public IActionResult GetCategories()
		{
			var category = _categoryRepository.GetCategories();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(category);
		}

		[HttpGet("Id/{CategoryId:int}", Name ="GetCategoryById")]
		[ProducesResponseType(200, Type = typeof(Category))]
		public IActionResult GetCategory(int CategoryId)
		{
			if (!_categoryRepository.CategoryExist(CategoryId))
			{
				return NotFound();
			}
			var category = _categoryRepository.GetCategory(CategoryId);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
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
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(category);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		// [Authorize]
		public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
		{
			if(categoryCreate == null)
			{
				return BadRequest(ModelState);
			}
			var category = _categoryRepository.GetCategories()
				.Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.ToUpper())
				.FirstOrDefault();
			if(category != null)
			{
				ModelState.AddModelError("", "Category already exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
            var mapCategory = _mapper.Map<Category>(categoryCreate);
          

            if (!_categoryRepository.CreateCategory(mapCategory))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500,ModelState);
			}
            categoryCreate.Id = mapCategory.Id;
            var url = Url.Link("GetCategoryById", new {CategoryId = categoryCreate.Id });
			return Created(url, categoryCreate);
		}

		[HttpPut("update")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		// [Authorize]
		public IActionResult UpdateCategory([FromBody] CategoryDto categoryUpdate)
		{

			if(categoryUpdate == null)
			{
				return BadRequest(ModelState);
			}
			if (!_categoryRepository.CategoryExist(categoryUpdate.Id))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
            var mapCategory = _mapper.Map<Category>(categoryUpdate);

            if (!_categoryRepository.UpdateCategory(mapCategory))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully updated");
		}

		[HttpDelete("{CategoryId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		// [Authorize]
		public IActionResult DeleteCategory(int CategoryId)
		{
			if (!_categoryRepository.CategoryExist(CategoryId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (! _categoryRepository.DeleteCategory(CategoryId))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully deleted");
		}
	}
}
