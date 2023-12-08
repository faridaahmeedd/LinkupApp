using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _categoryRepository;

		public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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

		[HttpGet("{CategoryId}")]
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

		[HttpGet("{CategoryName}/name")]
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
		public IActionResult CreateCategory([FromBody] Category categoryCreate)
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
			if (!_categoryRepository.CreateCategory(categoryCreate))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500,ModelState);
			}
			return Ok("Successfully created");
		}
	}
}
