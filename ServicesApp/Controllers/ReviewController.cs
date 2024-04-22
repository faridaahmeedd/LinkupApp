using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Dto.Reviews_Reports;
using AutoMapper;
using ServicesApp.APIs;
using Azure.Core;

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
        private readonly IServiceRequestRepository _serviceRequestRepository;


        public ReviewController(IReviewRepository IReviewRepository, IMapper mapper, 
            IProviderRepository providerRepository, ICustomerRepository customerRepository, IServiceRequestRepository serviceRequestRepository)
        {
            _ReviewRepository = IReviewRepository;
            _customerRepository = customerRepository;
            _providerRepository = providerRepository;
            _mapper = mapper;
            _serviceRequestRepository = serviceRequestRepository;
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
                    reviewDto.ReviewerName = "shrouk";
                  //  reviewDto.ReviewerName = review.Customer.FName + " " + review.Customer.LName;
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
                var Reviews = _ReviewRepository.GetReviewsOfProvider(ProviderId);
                var mappedReviews = Reviews.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);
                    //reviewDto.ReviewerName = "fefe";
                    reviewDto.ReviewerName = review.request.Customer.FName + " " + review.request.Customer.LName;
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
        public async Task<IActionResult> GetRating(string Id)
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
                var rating = await _ReviewRepository.CalculateAvgRating(Id);
                return Ok(rating);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpPost("Customer/{RequestId}")]
        public IActionResult CreateCustomerReview(int RequestId,  [FromBody] PostReviewDto ReviewCreate)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_serviceRequestRepository.ServiceExist(RequestId))
                {
                    return NotFound(ApiResponse.RequestNotFound);
                }
               
                mapReview.request = _serviceRequestRepository.GetService(RequestId);
                mapReview.ReviewerRole = "Provider";
                _ReviewRepository.CreateReview(mapReview);
              //  _ReviewRepository.Warning(CustomerId);

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

        [HttpPost("Provider/{RequestId}")]
        public IActionResult CreateProviderReview(int RequestId, [FromBody] PostReviewDto ReviewCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_serviceRequestRepository.ServiceExist(RequestId))
                {
                    return NotFound(ApiResponse.RequestNotFound);
                }

                mapReview.request = _serviceRequestRepository.GetService(RequestId);
                mapReview.ReviewerRole = "Customer";
                // _ReviewRepository.Warning(ProviderId);

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