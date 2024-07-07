using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using ServicesApp.Controllers;
using ServicesApp.Dto.Reviews_Reports;
using ServicesApp.Dto.Subcategory;
using ServicesApp.Dto.User;
using ServicesApp.Helper;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repositories;
using ServicesApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Tests.Controller
{
	public class ReviewControllerTests
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IProviderRepository _providerRepository;
		private readonly IServiceRequestRepository _serviceRequestRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;

		public ReviewControllerTests()
        {
			_customerRepository = A.Fake<ICustomerRepository>();
			_providerRepository = A.Fake<IProviderRepository>();
			_serviceRequestRepository = A.Fake<IServiceRequestRepository>();
			_reviewRepository = A.Fake<IReviewRepository>();
			_mapper = A.Fake<IMapper>();
		}

		[Fact]
		public async Task GetRating_CustomerFound_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var expectedRating = 4.5;   // Example expected rating
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _reviewRepository.CalculateAvgRating(customerId)).Returns(expectedRating);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository); 

			// Act
			var result = await _reviewController.GetRating(customerId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
			var rating = Assert.IsType<double>(okResult.Value);
			Assert.InRange(rating, 0, 5);       
		}

		[Fact]
		public async Task GetRating_UserNotFound_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistentUserId";
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);
			A.CallTo(() => _providerRepository.ProviderExist(customerId)).Returns(false);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.GetRating(customerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
		}

		[Fact]
		public async Task GetRating_RatingOutOfRange_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var invalidRating = 6.0; // Invalid rating, should still pass due to out-of-range check
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _providerRepository.ProviderExist(customerId)).Returns(false);
			A.CallTo(() => _reviewRepository.CalculateAvgRating(customerId)).Returns(invalidRating);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.GetRating(customerId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
		}

		[Fact]
		public async Task GetRating_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);
			_reviewController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = await _reviewController.GetRating(customerId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public void GetReviews_WhenCalled_ReturnsOk()
		{
			// Arrange
			var reviews = A.Fake<ICollection<Review>>();
			var mappedReviews = A.Fake<List<GetReviewDto>>();
			A.CallTo(() => _reviewRepository.GetReviews()).Returns(reviews);
			A.CallTo(() => _mapper.Map<List<GetReviewDto>>(reviews)).Returns(mappedReviews);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetReviews();

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetReviewById_ReviewExist_ReturnsOk()
		{
			// Arrange
			var reviewId = 1;  // Existing review Id
			var review = A.Fake<Review>();
			var mappedReview = A.Fake<GetReviewDto>();
			A.CallTo(() => _reviewRepository.ReviewExist(reviewId)).Returns(true);
			A.CallTo(() => _reviewRepository.GetReview(reviewId)).Returns(review);
			A.CallTo(() => _mapper.Map<GetReviewDto>(review)).Returns(mappedReview);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetReview(reviewId);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetReviewById_ReviewDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var reviewId = 1;  // Existing review Id
			var review = A.Fake<Review>();
			var mappedReview = A.Fake<GetReviewDto>();
			A.CallTo(() => _reviewRepository.ReviewExist(reviewId)).Returns(false);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetReview(reviewId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetCustomerReviews_CustomerExists_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var reviews = A.Fake<ICollection<Review>>();
			var reviewsList = A.Fake<List<GetReviewDto>>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _reviewRepository.GetReviewsOfCustomer(customerId)).Returns(reviews);
			A.CallTo(() => _mapper.Map<List<GetReviewDto>>(reviews)).Returns(reviewsList);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetCustomerReviews(customerId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetCustomerReviews_CustomerDoesNotExists_ReturnsOk()
		{
			// Arrange
			var customerId = "NonExistentCustomerId";
			var reviews = A.Fake<ICollection<Review>>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetCustomerReviews(customerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetCustomerReviews_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);
			_reviewController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = _reviewController.GetCustomerReviews(customerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public void GetProviderReviews_ProviderExists_ReturnsOk()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var reviews = A.Fake<ICollection<Review>>();
			var reviewsList = A.Fake<List<GetReviewDto>>();
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
			A.CallTo(() => _reviewRepository.GetReviewsOfProvider(providerId)).Returns(reviews);
			A.CallTo(() => _mapper.Map<List<GetReviewDto>>(reviews)).Returns(reviewsList);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetProviderReviews(providerId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetProviderReviews_ProviderDoesNotExists_ReturnsOk()
		{
			// Arrange
			var providerId = "NonExistentProviderId";
			var reviews = A.Fake<ICollection<Review>>();
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reviewController.GetProviderReviews(providerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetProviderReviews_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);
			_reviewController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = _reviewController.GetProviderReviews(providerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task CreateCustomerReview_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var requestId = 999; // Non-existing service ID
			var reviewDto = A.Fake<PostReviewDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(false);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateCustomerReview(reviewDto, requestId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
		}

		[Fact]
		public async Task CreateProviderReview_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var requestId = 999; // Non-existing service ID
			var reviewDto = A.Fake<PostReviewDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(false);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateProviderReview(reviewDto, requestId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
		}

		[Fact]
		public async Task CreateCustomerReview_UncompletedService_ReturnsBadRequest()
		{
			// Arrange
			var requestId = 1; // Existing service ID but not completed
			var reviewDto = A.Fake<PostReviewDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(false);
			A.CallTo(() => _reviewRepository.IsProviderAlreadyReviewed(requestId)).Returns(false);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateCustomerReview(reviewDto, requestId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
		}

		[Fact]
		public async Task CreateProviderReview_UncompletedService_ReturnsBadRequest()
		{
			// Arrange
			var requestId = 1; // Existing service ID but not completed
			var reviewDto = A.Fake<PostReviewDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(false);
			A.CallTo(() => _reviewRepository.IsCustomerAlreadyReviewed(requestId)).Returns(false);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateProviderReview(reviewDto, requestId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
		}

		[Fact]
		public async Task CreateCustomerReview_ServiceAlreadyReviewed_ReturnsBadRequest()
		{
			// Arrange
			var requestId = 1;
			var reviewDto = A.Fake<PostReviewDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(true);
			A.CallTo(() => _reviewRepository.IsProviderAlreadyReviewed(requestId)).Returns(true);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateCustomerReview(reviewDto, requestId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
		}

		[Fact]
		public async Task CreateProviderReview_ServiceAlreadyReviewed_ReturnsBadRequest()
		{
			// Arrange
			var requestId = 1; // Existing service ID but not completed
			var reviewDto = A.Fake<PostReviewDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(true);
			A.CallTo(() => _reviewRepository.IsCustomerAlreadyReviewed(requestId)).Returns(true);
			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateProviderReview(reviewDto, requestId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
		}

		[Fact]
		public async Task CreateCustomerReview_ValidRequest_ReturnsOk()
		{
			// Arrange
			var requestId = 1;
			var reviewDto = A.Fake<PostReviewDto>();
			var mappedReview = A.Fake<Review>();
			var request = A.Fake<ServiceRequest>();

			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(true);
			A.CallTo(() => _reviewRepository.IsProviderAlreadyReviewed(requestId)).Returns(false);
			A.CallTo(() => _mapper.Map<Review>(reviewDto)).Returns(mappedReview);
			A.CallTo(() => _serviceRequestRepository.GetService(requestId)).Returns(request);

			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateCustomerReview(reviewDto, requestId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
		}

		[Fact]
		public async Task CreateProviderReview_ValidRequest_ReturnsOk()
		{
			// Arrange
			var requestId = 1;
			var reviewDto = A.Fake<PostReviewDto>();
			var mappedReview = A.Fake<Review>();
			var request = A.Fake<ServiceRequest>();

			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(true);
			A.CallTo(() => _reviewRepository.IsCustomerAlreadyReviewed(requestId)).Returns(false);
			A.CallTo(() => _mapper.Map<Review>(reviewDto)).Returns(mappedReview);
			A.CallTo(() => _serviceRequestRepository.GetService(requestId)).Returns(request);

			var _reviewController = new ReviewController(_reviewRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = await _reviewController.CreateProviderReview(reviewDto, requestId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
		}
	}
}
