using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using AutoMapper;
using ServicesApp.Dto.Subcategory;
using System.Collections.Generic;
using ServicesApp.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

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
					return BadRequest(ApiResponses.NotValid);
				}
				var subcategories = _subcategoryRepository.GetSubcategories();
				var mapSubcategories = _mapper.Map<List<SubcategoryDto>>(subcategories);
				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("OfCategory/{CategoryId}")]
		public IActionResult GetSubcategories(int CategoryId)
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
				var subcategories = _subcategoryRepository.GetSubcategories(CategoryId);
				var mapSubcategories = _mapper.Map<List<SubcategoryDto>>(subcategories);
				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{SubcategoryId}")]
		public IActionResult GetSubcategory(int SubcategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_subcategoryRepository.SubcategoryExist(SubcategoryId))
				{
					return NotFound(ApiResponses.SubcategoryNotFound);
				}
				var subcategory = _subcategoryRepository.GetSubcategory(SubcategoryId);
				var mapSubcategories = _mapper.Map<SubcategoryDto>(subcategory);

				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{SubcategoryName:minlength(3)}")]
		public IActionResult GetSubcategory(string SubcategoryName)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var subcategory = _subcategoryRepository.GetSubcategory(SubcategoryName);
				if (subcategory == null)
				{
					return NotFound(ApiResponses.SubcategoryNotFound);
				}
				var mapSubcategories = _mapper.Map<SubcategoryDto>(subcategory);

				return Ok(mapSubcategories);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPost("{CategoryId}")]
		public IActionResult CreateSubcategory(int CategoryId, [FromBody] SubcategoryDto subcategoryCreate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				var subcategoryEn = _subcategoryRepository.GetSubcategory(subcategoryCreate.NameEn);
                var subcategoryAr = _subcategoryRepository.GetSubcategory(subcategoryCreate.NameAr);

                if (subcategoryAr != null || subcategoryEn != null)
				{
					return BadRequest(ApiResponses.SubcategoryAlreadyExist);
				}
				if (!_categoryRepository.CategoryExist(CategoryId))
				{
					return NotFound(ApiResponses.CategoryNotFound);
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
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut()]
		public IActionResult UpdateSubcategory([FromBody] SubcategoryDto subcategoryUpdate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_subcategoryRepository.SubcategoryExist(subcategoryUpdate.Id))
				{
					return NotFound(ApiResponses.SubcategoryNotFound);
				}
				var mapSubcategory = _mapper.Map<Subcategory>(subcategoryUpdate);

				_subcategoryRepository.UpdateSubcategory(mapSubcategory);
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpDelete("{SubcategoryId}")]
		public IActionResult DeleteSubcategory(int SubcategoryId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_subcategoryRepository.SubcategoryExist(SubcategoryId))
				{
					return NotFound(ApiResponses.SubcategoryNotFound);
				}
				_subcategoryRepository.DeleteSubcategory(SubcategoryId);
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
	}
}