using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Dto.Review;
using ServicesApp.Core.Models;
using AutoMapper;
using ServicesApp.APIs;
using ServicesApp.Dto.SubReview;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly IReviewRepository _ReviewRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository ReviewRepository , IMapper mapper)
        {
            _ReviewRepository = ReviewRepository;
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
				var categories = _ReviewRepository.GetCategories();
				var mapCategories = _mapper.Map<List<PostReviewDto>>(categories);
				return Ok(mapCategories);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{ReviewId:int}", Name ="GetReviewById")]
		public IActionResult GetReview(int ReviewId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_ReviewRepository.ReviewExist(ReviewId))
				{
					return NotFound(ApiResponse.ReviewNotFound);
				}
				var Review = _ReviewRepository.GetReview(ReviewId);
				var mapReview = _mapper.Map<PostReviewDto>(Review);
				return Ok(mapReview);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPost]
		public IActionResult CreateReview([FromBody] PostReviewDto ReviewCreate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var Review = _ReviewRepository.GetReview(ReviewCreate.Name);
				if (Review != null)
				{
					return BadRequest(ApiResponse.ReviewAlreadyExist);
				}
				var mapReview = _mapper.Map<Review>(ReviewCreate);

				_ReviewRepository.CreateReview(mapReview);
				return Ok(new
				{
					statusMsg = "success",
					message = "Review Created Successfully.",
					ReviewId = mapReview.Id,
				});
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
	}
}