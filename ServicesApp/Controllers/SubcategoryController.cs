using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using AutoMapper;
using ServicesApp.APIs;
using ServicesApp.Dto.Subcategory;
using System.Collections.Generic;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class SubcategoryController : ControllerBase
	{
		private readonly ISubcategoryRepository _subcategoryRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

        public SubcategoryController(ISubcategoryRepository subcategoryRepository , ICategoryRepository categoryRepository , IMapper mapper)
        {
            _subcategoryRepository = subcategoryRepository;
			_categoryRepository = categoryRepository;
			_mapper = mapper;
        }

		[HttpGet]
		public IActionResult GetSubcategories()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var subcategories = _subcategoryRepository.GetSubcategories();
				var mapSubcategories = _mapper.Map<List<SubcategoryDto>>(subcategories);
				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("OfCategory/{CategoryId}")]
		public IActionResult GetSubcategories(int CategoryId)
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
				var subcategories = _subcategoryRepository.GetSubcategories(CategoryId);
				var mapSubcategories = _mapper.Map<List<SubcategoryDto>>(subcategories);
				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{SubcategoryId:int}", Name = "GetSubcategoryById")]
		public IActionResult GetSubcategory(int SubcategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_subcategoryRepository.SubcategoryExist(SubcategoryId))
				{
					return NotFound(ApiResponse.SubcategoryNotFound);
				}
				var subcategory = _subcategoryRepository.GetSubcategory(SubcategoryId);
				var mapSubcategories = _mapper.Map<SubcategoryDto>(subcategory);
				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{SubcategoryName:minlength(3)}")]
		public IActionResult GetSubcategory(String SubcategoryName)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var subcategory = _subcategoryRepository.GetSubcategory(SubcategoryName);
				if (subcategory == null)
				{
					return NotFound(ApiResponse.SubcategoryNotFound);
				}
				var mapSubcategories = _mapper.Map<SubcategoryDto>(subcategory);
				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPost("{CategoryId}")]
		public IActionResult CreateSubcategory(int CategoryId, [FromBody] SubcategoryDto subcategoryCreate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var subcategory = _subcategoryRepository.GetSubcategory(subcategoryCreate.Name);
				if (subcategory != null)
				{
					return BadRequest(ApiResponse.SubcategoryAlreadyExist);
				}
				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponse.CategoryNotFound);
				}
				var mapSubcategory = _mapper.Map<Subcategory>(subcategoryCreate);
				mapSubcategory.Category = _categoryRepository.GetCategory(CategoryId);
				_subcategoryRepository.CreateSubcategory(mapSubcategory);
				return Ok(new
				{
					statusMsg = "success",
					message = "Subcategory Created Successfully.",
					SubcategoryId = mapSubcategory.Id,
				});
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut()]
		public IActionResult UpdateSubcategory([FromBody] SubcategoryDto subcategoryUpdate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_subcategoryRepository.SubcategoryExist(subcategoryUpdate.Id))
				{
					return NotFound(ApiResponse.SubcategoryNotFound);
				}
				var mapSubcategory = _mapper.Map<Subcategory>(subcategoryUpdate);

				_subcategoryRepository.UpdateSubcategory(mapSubcategory);
				return Ok(ApiResponse.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpDelete("{SubcategoryId}")]
		public IActionResult DeleteSubcategory(int SubcategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_subcategoryRepository.SubcategoryExist(SubcategoryId))
				{
					return NotFound(ApiResponse.SubcategoryNotFound);
				}
				_subcategoryRepository.DeleteSubcategory(SubcategoryId);
				return Ok(ApiResponse.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
	}
}