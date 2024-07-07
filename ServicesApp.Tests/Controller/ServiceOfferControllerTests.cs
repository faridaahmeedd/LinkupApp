using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Controllers;
using ServicesApp.Dto.Service;
using ServicesApp.Helper;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using Xunit;
using FakeItEasy;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;

namespace ServicesApp.Tests.Controller
{
    public class ServiceOfferControllerTests
    {
        private readonly IServiceOfferRepository _offerRepository;
        private readonly IServiceRequestRepository _requestRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly ITimeSlotsRepository _timeSlotsRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly ServiceOfferController _controller;

        public ServiceOfferControllerTests()
        {
            _offerRepository = A.Fake<IServiceOfferRepository>();
            _requestRepository = A.Fake<IServiceRequestRepository>();
            _providerRepository = A.Fake<IProviderRepository>();
            _timeSlotsRepository = A.Fake<ITimeSlotsRepository>();
            _reviewRepository = A.Fake<IReviewRepository>();
            _mapper = A.Fake<IMapper>();

            _controller = new ServiceOfferController(
                _requestRepository,
                _offerRepository,
                _providerRepository,
                _timeSlotsRepository,
                _reviewRepository,
                _mapper
            );
        }

        [Fact]
        public async Task GetOffers_ValidModel_ReturnsOk()
        {
            // Arrange
            A.CallTo(() => _offerRepository.GetOffers()).Returns(new List<ServiceOffer>());
            A.CallTo(() => _reviewRepository.CalculateAvgRating(A<string>._)).Returns(Task.FromResult(4.5));

            // Act
            var result = await _controller.GetOffers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<GetServiceOfferDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetOffer_ExistingOfferId_ReturnsOk()
        {
            // Arrange
            var offerId = 1;
            var serviceOfferDto = A.Fake<GetServiceOfferDto>();
            var serviceoffer = A.Fake<ServiceOffer>();
			serviceOfferDto.Id = offerId;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
            A.CallTo(() => _offerRepository.GetOffer(offerId)).Returns(serviceoffer);
            A.CallTo(() => _reviewRepository.CalculateAvgRating(serviceOfferDto.ProviderId)).Returns(Task.FromResult(4.5));
            A.CallTo(() => _mapper.Map<GetServiceOfferDto>(serviceoffer)).Returns(serviceOfferDto);

            // Act
            var result = await _controller.GetOffer(offerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var offer = Assert.IsAssignableFrom<GetServiceOfferDto>(okResult.Value);
            Assert.Equal(offerId, offer.Id);
        }

        [Fact]
        public async Task GetOffer_NonExistingOfferId_ReturnsNotFound()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

            // Act
            var result = await _controller.GetOffer(offerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
        }


        [Fact]
        public void CreateOffer_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "ModelState Error");

            // Act
            var result = _controller.CreateOffer("providerId", 1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
        }

        [Fact]
        public void CreateOffer_NonExistingProvider_ReturnsNotFound()
        {
            // Arrange
            var providerId = "NonExistingId";
            var requestId = 1;
            var serviceOffer = A.Fake<PostServiceOfferDto>();
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

            // Act
            var result = _controller.CreateOffer(providerId, requestId, serviceOffer);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
        }

        [Fact]
        public void CreateOffer_ValidModel_ReturnsOk()
        {
            // Arrange
            var providerId = "providerId";
            var requestId = 1;
            var serviceOfferDto = A.Fake<PostServiceOfferDto>();
            var serviceRequest = A.Fake<ServiceRequest>();
            var provider = A.Fake<Provider>();
            var serviceOffer = A.Fake<ServiceOffer>();
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
            A.CallTo(() => _requestRepository.ServiceExist(requestId)).Returns(true);
            A.CallTo(() => _requestRepository.TimeSlotsExistInService(requestId, serviceOfferDto.TimeSlotId)).Returns(true);
            A.CallTo(() => _offerRepository.ProviderAlreadyOffered(providerId, requestId)).Returns(false);
            A.CallTo(() => _mapper.Map<ServiceOffer>(serviceOfferDto)).Returns(serviceOffer);
            A.CallTo(() => _providerRepository.GetProvider(providerId)).Returns(provider);
            A.CallTo(() => _requestRepository.GetService(requestId)).Returns(serviceRequest);
            A.CallTo(() => _timeSlotsRepository.CheckConflict(serviceOffer)).Returns(true);
            A.CallTo(() => _offerRepository.CheckFeesRange(serviceOffer)).Returns(true);

            // Act
            var result = _controller.CreateOffer(providerId, requestId, serviceOfferDto);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
			Assert.Equal(serviceOffer.Id, responseObject["OfferId"]);
        }

        [Fact]
        public void UpdateOffer_ValidModel_ReturnsOk()
        {
            // Arrange
            var offerId = 1;
            var serviceOfferDto = A.Fake<PostServiceOfferDto>();
            var existingOffer = A.Fake<ServiceOffer>();
            existingOffer.Request = A.Fake<ServiceRequest>();
			serviceOfferDto.TimeSlotId = 1;
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
            A.CallTo(() => _offerRepository.GetOffer(offerId)).Returns(existingOffer);
            A.CallTo(() => _requestRepository.TimeSlotsExistInService(existingOffer.Request.Id, serviceOfferDto.TimeSlotId)).Returns(true);
            A.CallTo(() => _mapper.Map<ServiceOffer>(serviceOfferDto)).Returns(existingOffer);
            A.CallTo(() => _offerRepository.CheckFeesRange(existingOffer)).Returns(true);
            A.CallTo(() => _offerRepository.UpdateOffer(existingOffer)).Returns(true);

            // Act
            var result = _controller.UpdateOffer(offerId, serviceOfferDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(okResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
		}

        [Fact]
        public void UpdateOffer_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "ModelState Error");

            // Act
            var result = _controller.UpdateOffer(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
        }

        [Fact]
        public void UpdateOffer_NonExistingOfferId_ReturnsNotFound()
        {
            // Arrange
            var offerId = 1;
			var serviceOffer = A.Fake<PostServiceOfferDto>();
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

            // Act
            var result = _controller.UpdateOffer(offerId, serviceOffer);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
        }

        [Fact]
        public void AcceptOffer_ExistingOfferId_ReturnsOk()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
            A.CallTo(() => _offerRepository.AcceptOffer(offerId)).Returns(true);

            // Act
            var result = _controller.AcceptOffer(offerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ApiResponses.OfferAccepted, okResult.Value);
        }

        [Fact]
        public void AcceptOffer_NonExistingOfferId_ReturnsNotFound()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

            // Act
            var result = _controller.AcceptOffer(offerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
        }

        [Fact]
        public void DeclineOffer_ExistingOfferId_ReturnsOk()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
            A.CallTo(() => _offerRepository.DeclineOffer(offerId)).Returns(true);

            // Act
            var result = _controller.DeclineOffer(offerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ApiResponses.OfferDeclined, okResult.Value);
        }

        [Fact]
        public void DeclineOffer_NonExistingOfferId_ReturnsNotFound()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

            // Act
            var result = _controller.DeclineOffer(offerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
        }

        [Fact]
        public void DeleteOffer_ExistingOfferId_ReturnsOk()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
            A.CallTo(() => _offerRepository.DeleteOffer(offerId)).Returns(true);

            // Act
            var result = _controller.DeleteOffer(offerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ApiResponses.SuccessDeleted, okResult.Value);
        }

        [Fact]
        public void DeleteOffer_NonExistingOfferId_ReturnsNotFound()
        {
            // Arrange
            var offerId = 1;
            A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

            // Act
            var result = _controller.DeleteOffer(offerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
        }

        [Fact]
        public async Task GetOffersOfProvider_ExistingProviderId_ReturnsOk()
        {
            // Arrange
            var providerId = "ExistingProviderId";
            var offers = A.Fake<List<ServiceOffer>>();
			var serviceOfferDto = A.Fake<List<GetServiceOfferDto>>();
			var serviceoffer = A.Fake<ServiceOffer>();
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
            A.CallTo(() => _offerRepository.GetOfffersOfProvider(providerId)).Returns(offers);
            A.CallTo(() => _reviewRepository.CalculateAvgRating(providerId)).Returns(Task.FromResult(4.5));
            A.CallTo(() => _mapper.Map<List<GetServiceOfferDto>>(offers)).Returns(serviceOfferDto);

            // Act
            var result = await _controller.GetOffersOfProvider(providerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<GetServiceOfferDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetOffersOfProvider_NonExistingProviderId_ReturnsNotFound()
        {
            // Arrange
            var providerId = "NonExistingProviderId";
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

            // Act
            var result = await _controller.GetOffersOfProvider(providerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
        }

        [Fact]
        public void ProviderCanOffer_ExistingProviderAndRequest_ReturnsOk()
        {
            // Arrange
            var providerId = "ExistingProviderId";
            var requestId = 1;
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
            A.CallTo(() => _requestRepository.ServiceExist(requestId)).Returns(true);
            A.CallTo(() => _offerRepository.ProviderAlreadyOffered(providerId, requestId)).Returns(false);

            // Act
            var result = _controller.ProviderCanOffer(providerId, requestId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ApiResponses.ProviderCanOffer, okResult.Value);
        }

        [Fact]
        public void ProviderCanOffer_NonExistingProvider_ReturnsNotFound()
        {
            // Arrange
            var providerId = "NonExistingProviderId";
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

            // Act
            var result = _controller.ProviderCanOffer(providerId, 1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
        }

        [Fact]
        public void ProviderCanOffer_NonExistingRequestId_ReturnsNotFound()
        {
            // Arrange
            var providerId = "ExistingProviderId";
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
            A.CallTo(() => _requestRepository.ServiceExist(1)).Returns(false);

            // Act
            var result = _controller.ProviderCanOffer(providerId, 1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
        }

        [Fact]
        public void ProviderCanOffer_AlreadyOffered_ReturnsBadRequest()
        {
            // Arrange
            var providerId = "ExistingProviderId";
            var requestId = 1;
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
            A.CallTo(() => _requestRepository.ServiceExist(requestId)).Returns(true);
            A.CallTo(() => _offerRepository.ProviderAlreadyOffered(providerId, requestId)).Returns(true);

            // Act
            var result = _controller.ProviderCanOffer(providerId, requestId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ApiResponses.AlreadyOffered, badRequestResult.Value);
        }

        [Fact]
        public void GetCalendarByProvider_ExistingProviderId_ReturnsOk()
        {
            // Arrange
            var providerId = "ExistingProviderId";
            var calendarDtos = A.Fake<List<GetCalendarDto>>();
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
            A.CallTo(() => _offerRepository.GetCalendarDetails(providerId)).Returns(calendarDtos);

            // Act
            var result = _controller.GetCalendarByProvider(providerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCalendarDtos = Assert.IsAssignableFrom<List<GetCalendarDto>>(okResult.Value);
            Assert.Same(calendarDtos, returnedCalendarDtos);
        }

        [Fact]
        public void GetCalendarByProvider_NonExistingProviderId_ReturnsNotFound()
        {
            // Arrange
            var providerId = "NonExistingProviderId";
            A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

            // Act
            var result = _controller.GetCalendarByProvider(providerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
        }

		[Fact]
		public void AssignProvider_InvalidModelState_ReturnsBadRequest()
		{
			// Arrange
			var offerId = 1;
			var providerId = "providerId";
			_controller.ModelState.AddModelError("Error", "Model error");

			// Act
			var result = _controller.AssignProvider(offerId, providerId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
		}

		[Fact]
		public void AssignProvider_OfferNotFound_ReturnsNotFound()
		{
			// Arrange
			var offerId = 1;
			var providerId = "providerId";
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

			// Act
			var result = _controller.AssignProvider(offerId, providerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
		}

		[Fact]
		public void AssignProvider_ProviderNotFound_ReturnsNotFound()
		{
			// Arrange
			var offerId = 1;
			var providerId = "providerId";
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

			// Act
			var result = _controller.AssignProvider(offerId, providerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
		}

		[Fact]
		public void AssignProvider_FailedToUpdate_ReturnsStatusCode500()
		{
			// Arrange
			var offerId = 1;
			var providerId = "providerId";
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
			A.CallTo(() => _offerRepository.AdminAssignProvider(offerId, providerId)).Returns(false);

			// Act
			var result = _controller.AssignProvider(offerId, providerId);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal(ApiResponses.FailedToUpdate, statusCodeResult.Value);
		}

		[Fact]
		public void AssignProvider_ValidRequest_ReturnsOk()
		{
			// Arrange
			var offerId = 1;
			var providerId = "providerId";
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
			A.CallTo(() => _offerRepository.AdminAssignProvider(offerId, providerId)).Returns(true);

			// Act
			var result = _controller.AssignProvider(offerId, providerId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
			Assert.Equal(ApiResponses.SuccessUpdated, okResult.Value);
		}

		[Fact]
		public void AssignProvider_Exception_ReturnsStatusCode500()
		{
			// Arrange
			var offerId = 1;
			var providerId = "providerId";
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Throws(new Exception());

			// Act
			var result = _controller.AssignProvider(offerId, providerId);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal(ApiResponses.SomethingWrong, statusCodeResult.Value);
		}

		[Fact]
		public void ApproveAdminOffer_OfferNotFound_ReturnsNotFound()
		{
			// Arrange
			var offerId = 1;
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(false);

			// Act
			var result = _controller.ApproveAdminOffer(offerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
		}

		[Fact]
		public void ApproveAdminOffer_FailedToUpdate_ReturnsStatusCode500()
		{
			// Arrange
			var offerId = 1;
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
			A.CallTo(() => _offerRepository.ApproveAdminOffer(offerId)).Returns(false);

			// Act
			var result = _controller.ApproveAdminOffer(offerId);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal(ApiResponses.FailedToUpdate, statusCodeResult.Value);
		}

		[Fact]
		public void ApproveAdminOffer_ValidRequest_ReturnsOk()
		{
			// Arrange
			var offerId = 1;
			A.CallTo(() => _offerRepository.OfferExist(offerId)).Returns(true);
			A.CallTo(() => _offerRepository.ApproveAdminOffer(offerId)).Returns(true);

			// Act
			var result = _controller.ApproveAdminOffer(offerId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
			Assert.Equal(ApiResponses.SuccessUpdated, okResult.Value);
		}
	}
}
