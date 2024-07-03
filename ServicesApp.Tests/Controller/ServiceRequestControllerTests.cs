using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServicesApp.Controllers;
using ServicesApp.Dto.Service;
using ServicesApp.Helper;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Tests.Controller
{
	public class ServiceRequestControllerTests
	{
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly ISubcategoryRepository _subcategoryRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IProviderRepository _providerRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;
		private readonly ServiceRequestController _controller;

		public ServiceRequestControllerTests()
		{
			_serviceRepository = A.Fake<IServiceRequestRepository>();
			_subcategoryRepository = A.Fake<ISubcategoryRepository>();
			_customerRepository = A.Fake<ICustomerRepository>();
			_providerRepository = A.Fake<IProviderRepository>();
			_reviewRepository = A.Fake<IReviewRepository>();
			_mapper = A.Fake<IMapper>();

			_controller = new ServiceRequestController(
				_serviceRepository,
				_subcategoryRepository,
				_customerRepository,
				_providerRepository,
				_reviewRepository,
				_mapper
			);
		}

		[Fact]
		public void GetServices_ValidModel_ReturnsOk()
		{
			// Arrange
			A.CallTo(() => _serviceRepository.GetServices()).Returns(new List<ServiceRequest>());

			// Act
			var result = _controller.GetServices();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsAssignableFrom<List<GetServiceRequestDto>>(okResult.Value);
		}

		[Fact]
		public void GetService_ExistingServiceId_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var serviceDto = A.Fake<GetServiceRequestDto>();
			var service = A.Fake<ServiceRequest>();
			serviceDto.Id = serviceId;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetService(serviceId)).Returns(service);
			A.CallTo(() => _mapper.Map<GetServiceRequestDto>(service)).Returns(serviceDto);

			// Act
			var result = _controller.GetService(serviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var serviceResult = Assert.IsAssignableFrom<GetServiceRequestDto>(okResult.Value);
			Assert.Equal(serviceId, serviceResult.Id);
		}

		[Fact]
		public void GetService_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.GetService(serviceId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void GetServicesByCustomer_ExistingCustomerId_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistingCustomerId";
			var services = A.Fake<List<ServiceRequest>>();
			var serviceDtos = A.Fake<List<GetServiceRequestDto>>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetServicesByCustomer(customerId)).Returns(services);
			A.CallTo(() => _mapper.Map<List<GetServiceRequestDto>>(services)).Returns(serviceDtos);

			// Act
			var result = _controller.GetServicesByCustomer(customerId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsAssignableFrom<List<GetServiceRequestDto>>(okResult.Value);
		}

		[Fact]
		public void GetServicesByCustomer_NonExistingCustomerId_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistingCustomerId";
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			// Act
			var result = _controller.GetServicesByCustomer(customerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
		}

		[Fact]
		public void GetUncompletedServices_ValidModel_ReturnsOk()
		{
			// Arrange
			A.CallTo(() => _serviceRepository.GetUncompletedServices()).Returns(new List<ServiceRequest>());

			// Act
			var result = _controller.GetUncompletedServices();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsAssignableFrom<List<GetServiceRequestDto>>(okResult.Value);
		}

		[Fact]
		public void CompleteService_ExistingServiceId_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);

			// Act
			var result = _controller.CompleteService(serviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(ApiResponses.ServiceCompletedSuccess, okResult.Value);
		}

		[Fact]
		public void CompleteService_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.CompleteService(serviceId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void CreateService_ValidModel_ReturnsOk()
		{
			// Arrange
			var customerId = "CustomerId";
			var subcategoryId = 1;
			var serviceRequestDto = A.Fake<PostServiceRequestDto>();
			var serviceRequest = A.Fake<ServiceRequest>();
			var subcategory = A.Fake<Subcategory>();
			var customer = A.Fake<Customer>();
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryId)).Returns(true);
			A.CallTo(() => _subcategoryRepository.GetSubcategory(subcategoryId)).Returns(subcategory);
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _customerRepository.GetCustomer(customerId)).Returns(customer);
			A.CallTo(() => _mapper.Map<ServiceRequest>(serviceRequestDto)).Returns(serviceRequest);

			// Act
			var result = _controller.CreateService(customerId, subcategoryId, serviceRequestDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(okResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
			Assert.Equal(serviceRequest.Id, responseObject["serviceId"]);
		}

		[Fact]
		public void CreateService_NonExistingSubcategory_ReturnsNotFound()
		{
			// Arrange
			var customerId = "CustomerId";
			var subcategoryId = 1;
			var serviceRequestDto = A.Fake<PostServiceRequestDto>();
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryId)).Returns(false);

			// Act
			var result = _controller.CreateService(customerId, subcategoryId, serviceRequestDto);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.SubcategoryNotFound, notFoundResult.Value);
		}

		[Fact]
		public void CreateService_NonExistingCustomer_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistingCustomerId";
			var subcategoryId = 1;
			var serviceRequestDto = A.Fake<PostServiceRequestDto>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			// Act
			var result = _controller.CreateService(customerId, subcategoryId, serviceRequestDto);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public void UpdateService_ValidModel_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var serviceRequestDto = A.Fake<PostServiceRequestDto>();
			var serviceRequest = A.Fake<ServiceRequest>();
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _mapper.Map<ServiceRequest>(serviceRequestDto)).Returns(serviceRequest);
			A.CallTo(() => _serviceRepository.UpdateService(serviceRequest)).Returns(true);

			// Act
			var result = _controller.UpdateService(serviceId, serviceRequestDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(ApiResponses.SuccessUpdated, okResult.Value);
		}

		[Fact]
		public void UpdateService_InvalidModel_ReturnsBadRequest()
		{
			// Arrange
			_controller.ModelState.AddModelError("Error", "ModelState Error");

			// Act
			var result = _controller.UpdateService(1, null);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
		}

		[Fact]
		public void UpdateService_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.UpdateService(serviceId, A.Fake<PostServiceRequestDto>());

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void DeleteService_ExistingServiceId_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.DeleteService(serviceId)).Returns(true);

			// Act
			var result = _controller.DeleteService(serviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(ApiResponses.SuccessDeleted, okResult.Value);
		}

		[Fact]
		public void DeleteService_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.DeleteService(serviceId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void DeleteService_FailedToDelete_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.DeleteService(serviceId)).Returns(false);

			// Act
			var result = _controller.DeleteService(serviceId);

			// Assert
			var okResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.FailedToDelete, okResult.Value);
		}

		[Fact]
		public async Task GetUndeclinedOffersOfService_ExistingServiceId_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var offers = A.Fake<List<ServiceOffer>>();
			var offerDtos = A.Fake<List<GetServiceOfferDto>>();
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetUndeclinedOffersOfService(serviceId)).Returns(offers);
			A.CallTo(() => _mapper.Map<List<GetServiceOfferDto>>(offers)).Returns(offerDtos);
			A.CallTo(() => _reviewRepository.CalculateAvgRating(A<string>._)).Returns(Task.FromResult(4.5)); 

			// Act
			var result = await _controller.GetUndeclinedOffersOfService(serviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var offerResults = Assert.IsAssignableFrom<List<GetServiceOfferDto>>(okResult.Value);
		}

		[Fact]
		public async Task GetUndeclinedOffersOfService_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = await _controller.GetUndeclinedOffersOfService(serviceId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public async Task GetAcceptedOffer_ExistingServiceId_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var acceptedOffer = A.Fake<ServiceOffer>();
			var offerDto = A.Fake<GetServiceOfferDto>();
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(acceptedOffer);
			A.CallTo(() => _mapper.Map<GetServiceOfferDto>(acceptedOffer)).Returns(offerDto);
			A.CallTo(() => _reviewRepository.CalculateAvgRating(A<string>._)).Returns(Task.FromResult(4.5)); // Example rating

			// Act
			var result = await _controller.GetAcceptedOffer(serviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var offerResult = Assert.IsAssignableFrom<GetServiceOfferDto>(okResult.Value);
			Assert.Equal(offerDto.ProviderId, offerResult.ProviderId);
		}

		[Fact]
		public async Task GetAcceptedOffer_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(null);

			// Act
			var result = await _controller.GetAcceptedOffer(serviceId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.OfferNotFound, notFoundResult.Value);
		}

		[Fact]
		public void UpdateServiceSubcategory_ValidModel_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var subcategoryName = "NewSubcategory";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryName)).Returns(true);
			A.CallTo(() => _serviceRepository.UpdateUnknownSubcategory(serviceId, subcategoryName)).Returns(true);

			// Act
			var result = _controller.UpdateServiceSubcategory(serviceId, subcategoryName);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(ApiResponses.SuccessUpdated, okResult.Value);
		}

		[Fact]
		public void UpdateServiceSubcategory_NonExistingServiceId_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			var subcategoryName = "NewSubcategory";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.UpdateServiceSubcategory(serviceId, subcategoryName);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void UpdateServiceSubcategory_NonExistingSubcategory_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			var subcategoryName = "NonExistingSubcategory";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _subcategoryRepository.SubcategoryExist(subcategoryName)).Returns(false);

			// Act
			var result = _controller.UpdateServiceSubcategory(serviceId, subcategoryName);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.SubcategoryNotFound, notFoundResult.Value);
		}

		[Fact]
		public void GetCalendarByCustomer_ExistingCustomerId_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistingCustomerId";
			var calendarDtos = A.Fake<List<GetCalendarDto>>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetCalendarDetails(customerId)).Returns(calendarDtos);

			// Act
			var result = _controller.GetCalendarByCustomer(customerId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsAssignableFrom<List<GetCalendarDto>>(okResult.Value);
		}

		[Fact]
		public void GetCalendarByCustomer_NonExistingCustomerId_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistingCustomerId";
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			// Act
			var result = _controller.GetCalendarByCustomer(customerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.UserNotFound, notFoundResult.Value);
		}

		[Fact]
		public void CreateRequestAfterExamination_InvalidModelState_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			_controller.ModelState.AddModelError("Error", "Model error");

			// Act
			var result = _controller.CreateRequestAfterExamination(serviceId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
		}

		[Fact]
		public void CreateRequestAfterExamination_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.CreateRequestAfterExamination(serviceId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void CreateRequestAfterExamination_NotExamination_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var acceptedOffer = A.Fake<ServiceOffer>();
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(acceptedOffer);

			// Act
			var result = _controller.CreateRequestAfterExamination(serviceId);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.NotExamination, badRequestResult.Value);
		}

		[Fact]
		public void CreateRequestAfterExamination_Exception_ReturnsStatusCode500()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Throws(new Exception());

			// Act
			var result = _controller.CreateRequestAfterExamination(serviceId);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal(ApiResponses.SomethingWrong, statusCodeResult.Value);
		}

		[Fact]
		public void AddExaminationComment_InvalidModelState_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var comment = "Test comment";
			_controller.ModelState.AddModelError("Error", "Model error");

			// Act
			var result = _controller.AddExaminationComment(serviceId, comment);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
		}

		[Fact]
		public void AddExaminationComment_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			var comment = "Test comment";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.AddExaminationComment(serviceId, comment);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void AddExaminationComment_NotExamination_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var comment = "Test comment";
			var acceptedOffer = A.Fake<ServiceOffer>();
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(acceptedOffer);

			// Act
			var result = _controller.AddExaminationComment(serviceId, comment);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.NotExamination, badRequestResult.Value);
		}

		[Fact]
		public void AddExaminationComment_FailedToUpdate_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var comment = "Test comment";
			ServiceOffer acceptedOffer = A.Fake<ServiceOffer>();
			acceptedOffer.Examination = true;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(acceptedOffer);
			A.CallTo(() => _serviceRepository.AddExaminationComment(serviceId, comment)).Returns(false);

			// Act
			var result = _controller.AddExaminationComment(serviceId, comment);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.FailedToUpdate, badRequestResult.Value);
		}

		[Fact]
		public void AddExaminationComment_ValidRequest_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var comment = "Test comment";
			ServiceOffer acceptedOffer = A.Fake<ServiceOffer>();
			acceptedOffer.Examination = true;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(acceptedOffer);
			A.CallTo(() => _serviceRepository.AddExaminationComment(serviceId, comment)).Returns(true);

			// Act
			var result = _controller.AddExaminationComment(serviceId, comment);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
			Assert.Equal(ApiResponses.SuccessUpdated, okResult.Value);
		}

		[Fact]
		public void AddEmergency_InvalidModelState_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var emergencyType = "Fire";
			_controller.ModelState.AddModelError("Error", "Model error");

			// Act
			var result = _controller.AddEmergency(serviceId, emergencyType);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.NotValid, badRequestResult.Value);
		}

		[Fact]
		public void AddEmergency_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			var emergencyType = "Fire";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = _controller.AddEmergency(serviceId, emergencyType);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal(ApiResponses.RequestNotFound, notFoundResult.Value);
		}

		[Fact]
		public void AddEmergency_FailedToUpdate_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var emergencyType = "Fire";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.AddEmergency(serviceId, emergencyType)).Returns(false);

			// Act
			var result = _controller.AddEmergency(serviceId, emergencyType);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(400, badRequestResult.StatusCode);
			Assert.Equal(ApiResponses.FailedToUpdate, badRequestResult.Value);
		}

		[Fact]
		public void AddEmergency_ValidRequest_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var emergencyType = "Fire";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.AddEmergency(serviceId, emergencyType)).Returns(true);

			// Act
			var result = _controller.AddEmergency(serviceId, emergencyType);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
			Assert.Equal(ApiResponses.SuccessUpdated, okResult.Value);
		}
	}
}
