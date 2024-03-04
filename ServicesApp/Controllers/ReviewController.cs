using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Dto.Reviews_Reports;
using ServicesApp.Core.Models;
using AutoMapper;
using ServicesApp.APIs;
using Microsoft.AspNetCore.Identity;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly IReviewRepository _ReviewRepository;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProviderRepository _providerRepository;

        public ReviewController(IReviewRepository IReviewRepository, IMapper mapper, IProviderRepository providerRepository, ICustomerRepository customerRepository)
        {
            _ReviewRepository = IReviewRepository;
			_customerRepository = customerRepository;
			_providerRepository = providerRepository;
			_mapper = mapper;
			
        }

		[HttpGet]
		public IActionResult GetReviews()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var reviews = _ReviewRepository.GetReviews();
				var mapreviews = _mapper.Map<List<GetReviewDto>>(reviews);
				return Ok(mapreviews);
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
				var mapReview = _mapper.Map<GetReviewDto>(Review);
				return Ok(mapReview);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
        [HttpGet("GetCustomerReviews/{customerId}")]
        public IActionResult GetCustomerReviews(string customerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_customerRepository.CustomerExist(customerId))
                {
                    return NotFound(ApiResponse.ReviewNotFound);
                }
                var Review = _ReviewRepository.GetReviewsOfCustomer(customerId);
                var mappedReviews = Review.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);

                    // Set the ReviewerName based on the customer's name
                    reviewDto.ReviewerName = review.Customer.FName;

                    return reviewDto;
                }).ToList();
                // var mapReview = _mapper.Map<List<GetReviewDto>>(Review);
                return Ok(mappedReviews);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
        [HttpGet("GetProviderReviews/{providerId}")]
        public IActionResult GetProviderReviews(string providerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_providerRepository.ProviderExist(providerId))
                {
                    return NotFound(ApiResponse.ReviewNotFound);
                }
                var Review = _ReviewRepository.GetReviewsOfProvider(providerId);
                var mappedReviews = Review.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);

                    // Set the ReviewerName based on the customer's name
                    reviewDto.ReviewerName = review.Provider.FName;

                    return reviewDto;
                }).ToList();
                // var mapReview = _mapper.Map<List<GetReviewDto>>(Review);
                return Ok(mappedReviews);
             
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpPost("ReviewCustomer")]
		public IActionResult CreateCustomerReview([FromBody] PostReviewDto ReviewCreate, string customerId , string providerId)
         
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}

				var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_customerRepository.CustomerExist(customerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(providerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReview.Customer = _customerRepository.GetCustomer(customerId);
                mapReview.Provider = _providerRepository.GetProvider(providerId);

                mapReview.ReviewerName = mapReview.Provider.FName;

                mapReview.ReviewerRole = "Provider";
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



        [HttpPost("reviewProvider")]
        public IActionResult CreateProviderReview([FromBody] PostReviewDto ReviewCreate, string customerId, string providerId)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_customerRepository.CustomerExist(customerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(providerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReview.Customer = _customerRepository.GetCustomer(customerId);
                mapReview.Provider = _providerRepository.GetProvider(providerId);

                mapReview.ReviewerName = mapReview.Customer.FName;

                mapReview.ReviewerRole = "Customer";

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