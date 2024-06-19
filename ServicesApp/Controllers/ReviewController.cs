using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Dto.Reviews_Reports;
using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Repositories;
using ServicesApp.Helper;

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
            IProviderRepository providerRepository, ICustomerRepository customerRepository, 
            IServiceRequestRepository serviceRequestRepository)
        {
            _ReviewRepository = IReviewRepository;
            _customerRepository = customerRepository;
            _providerRepository = providerRepository;
			_serviceRequestRepository = serviceRequestRepository;
			_mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetReviews()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                var reviews = _ReviewRepository.GetReviews();
                var mapreviews = _mapper.Map<List<GetReviewDto>>(reviews);
                return Ok(mapreviews);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("{ReviewId}")]
        public IActionResult GetReview(int ReviewId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_ReviewRepository.ReviewExist(ReviewId))
                {
                    return NotFound(ApiResponses.ReviewNotFound);
                }
                var Review = _ReviewRepository.GetReview(ReviewId);
                var mapReview = _mapper.Map<GetReviewDto>(Review);
                return Ok(mapReview);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("CustomerReviews/{CustomerId}")]
        public IActionResult GetCustomerReviews(string CustomerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponses.UserNotFound);
                }
                var Review = _ReviewRepository.GetReviewsOfCustomer(CustomerId);
                var mappedReviews = Review.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);
                    return reviewDto;
                }).ToList();
                return Ok(mappedReviews);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("ProviderReviews/{ProviderId}")]
        public IActionResult GetProviderReviews(string ProviderId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponses.UserNotFound);
                }
                var Reviews = _ReviewRepository.GetReviewsOfProvider(ProviderId);
                var mappedReviews = Reviews.Select(review =>
                {
                    var reviewDto = _mapper.Map<GetReviewDto>(review);
                    return reviewDto;
                }).ToList();

                return Ok(mappedReviews);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("Rating/{Id}")]
        public async Task<IActionResult> GetRating(string Id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_customerRepository.CustomerExist(Id) && !_providerRepository.ProviderExist(Id))
                {
                    return NotFound(ApiResponses.UserNotFound);
                }
                var rating = await _ReviewRepository.CalculateAvgRating(Id);
				if (rating < 0 || rating > 5)
				{
					return BadRequest(ApiResponses.InvalidRating);
				}
				return Ok(rating);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpPost("Customer/{RequestId}")]
        public async Task<IActionResult> CreateCustomerReview(int RequestId, [FromBody] PostReviewDto ReviewCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }

                var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_serviceRequestRepository.ServiceExist(RequestId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }

				if (!_ReviewRepository.CheckRequestOfReviewCompleted(RequestId))
				{
					return BadRequest(ApiResponses.UncompletedService);
				}

				mapReview.Request = _serviceRequestRepository.GetService(RequestId);
				var acceptedOffer = _serviceRequestRepository.GetAcceptedOffer(mapReview.Request.Id);
				mapReview.ReviewerName = acceptedOffer?.Provider?.FName + " " + acceptedOffer?.Provider?.LName;

				if (_ReviewRepository.GetReviewsOfRequest(mapReview.Request.Id).Any(review => review.ReviewerRole == "Provider"))
				{
                    return BadRequest(ApiResponses.ServiceAlreadyReviewed);
                }
                mapReview.ReviewerRole = "Provider";
                _ReviewRepository.CreateReview(mapReview);
                
                await _ReviewRepository.Warning(mapReview.Request.Customer.Id);

                return Ok(new
                {
                    statusMsg = "success",
                    message = "Review Created Successfully.",
                    ReviewId = mapReview.Id,
                });
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpPost("Provider/{RequestId}")]
        public async Task<IActionResult> CreateProviderReview(int RequestId, [FromBody] PostReviewDto ReviewCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }

                var mapReview = _mapper.Map<Review>(ReviewCreate);
                if (!_serviceRequestRepository.ServiceExist(RequestId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }
                if (!_ReviewRepository.CheckRequestOfReviewCompleted(RequestId))
                {
                    return BadRequest(ApiResponses.UncompletedService);
				}

                mapReview.Request = _serviceRequestRepository.GetService(RequestId);
				mapReview.ReviewerName = mapReview.Request.Customer.FName + " " + mapReview.Request.Customer.LName;

                if (_ReviewRepository.GetReviewsOfRequest(mapReview.Request.Id).Any(review => review.ReviewerRole == "Customer"))
                {
                    return BadRequest(ApiResponses.ServiceAlreadyReviewed);
                }
                mapReview.ReviewerRole = "Customer";
                 _ReviewRepository.CreateReview(mapReview);
                var acceptedOffer = _serviceRequestRepository.GetAcceptedOffer(mapReview.Request.Id);

                await _ReviewRepository.Warning(acceptedOffer?.Provider?.Id);
                return Ok(new
                {
                    statusMsg = "success",
                    message = "Review Created Successfully.",
                    ReviewId = mapReview.Id,
                });
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }
    }
}