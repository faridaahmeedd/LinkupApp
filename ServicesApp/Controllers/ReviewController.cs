using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Dto.Reviews_Reports;
using AutoMapper;
using ServicesApp.APIs;

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

        [HttpGet("{ReviewId}")]
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

        [HttpGet("CustomerReviews/{CustomerId}")]
        public IActionResult GetCustomerReviews(string CustomerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                var Review = _ReviewRepository.GetReviewsOfCustomer(CustomerId);
                var mappedReviews = Review.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);
                    reviewDto.ReviewerName = review.Customer.FName + " " + review.Customer.LName;
                    return reviewDto;
                }).ToList();
                return Ok(mappedReviews);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpGet("ProviderReviews/{ProviderId}")]
        public IActionResult GetProviderReviews(string ProviderId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponse.ReviewNotFound);
                }
                var Review = _ReviewRepository.GetReviewsOfProvider(ProviderId);
                var mappedReviews = Review.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);
                    reviewDto.ReviewerName = review.Provider.FName + " " + review.Provider.LName;
					return reviewDto;
                }).ToList();

                return Ok(mappedReviews);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

		[HttpGet("Rating/{Id}")]
		public IActionResult GetRating(string Id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_customerRepository.CustomerExist(Id) && !_providerRepository.ProviderExist(Id))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				var rating = _ReviewRepository.CalculateAvgRating(Id);
				return Ok(rating);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPost("Customer/{CustomerId}/{ProviderId}")]
		public IActionResult CreateCustomerReview(string CustomerId, string ProviderId, [FromBody] PostReviewDto ReviewCreate)
         
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}

				var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReview.Customer = _customerRepository.GetCustomer(CustomerId);
                mapReview.Provider = _providerRepository.GetProvider(ProviderId);

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

        [HttpPost("Provider/{CustomerId}/{ProviderId}")]
        public IActionResult CreateProviderReview(string CustomerId, string ProviderId, [FromBody] PostReviewDto ReviewCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReview.Customer = _customerRepository.GetCustomer(CustomerId);
                mapReview.Provider = _providerRepository.GetProvider(ProviderId);
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