using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
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
	public class PayPalControllerTests
	{
		private readonly IPayPalRepository _payPalRepository;
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly PayPalController _payPalController;

		public PayPalControllerTests()
		{
			_payPalRepository = A.Fake<IPayPalRepository>();
			_serviceRepository = A.Fake<IServiceRequestRepository>();
			_payPalController = new PayPalController(_payPalRepository, _serviceRepository);
		}

		[Fact]
		public async Task CreatePayment_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = await _payPalController.CreatePayment(serviceId);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, actionResult.Value);
		}

		[Fact]
		public async Task CreatePayment_AcceptedOfferNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(null);

			// Act
			var result = await _payPalController.CreatePayment(serviceId);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.OfferNotFound, actionResult.Value);
		}

		[Fact]
		public async Task CreatePayment_ServiceAlreadyPaid_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var request = A.Fake<ServiceRequest>();
			var offer = A.Fake<ServiceOffer>();
			request.PaymentStatus = "Paid";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(offer);
			A.CallTo(() => _serviceRepository.GetService(serviceId)).Returns(request);

			// Act
			var result = await _payPalController.CreatePayment(serviceId);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.PaidAlready, actionResult.Value);
		}

		[Fact]
		public async Task CreatePayment_PaymentSuccessful_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var approvalLink = "http://approvallink.com";
			var request = A.Fake<ServiceRequest>();
			var offer = A.Fake<ServiceOffer>();
			request.PaymentStatus = "Pending";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(offer);
			A.CallTo(() => _serviceRepository.GetService(serviceId)).Returns(request);
			A.CallTo(() => _payPalRepository.CreatePayment(serviceId)).Returns(Task.FromResult(approvalLink));

			// Act
			var result = await _payPalController.CreatePayment(serviceId);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(approvalLink, actionResult.Value);
		}

		[Fact]
		public async Task CreatePayment_PaymentFailed_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var request = A.Fake<ServiceRequest>();
			var offer = A.Fake<ServiceOffer>();
			request.PaymentStatus = "Pending";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(offer);
			A.CallTo(() => _serviceRepository.GetService(serviceId)).Returns(request);
			A.CallTo(() => _payPalRepository.CreatePayment(serviceId)).Returns(Task.FromResult<string>(null));

			// Act
			var result = await _payPalController.CreatePayment(serviceId);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.PaymentError, actionResult.Value);
		}

		[Fact]
		public async Task ExecutePayment_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = await _payPalController.ExecutePayment(serviceId, "paymentId", "token", "payerId");

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, actionResult.Value);
		}

		[Fact]
		public async Task ExecutePayment_PaymentApproved_ReturnsOkWithSuccessUrl()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _payPalRepository.ExecutePayment(serviceId, "paymentId", "token", "payerId")).Returns(Task.FromResult("approved"));

			// Act
			var result = await _payPalController.ExecutePayment(serviceId, "paymentId", "token", "payerId");

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ExecutePayment_PaymentNotApproved_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _payPalRepository.ExecutePayment(serviceId, "paymentId", "token", "payerId")).Returns(Task.FromResult("failed"));

			// Act
			var result = await _payPalController.ExecutePayment(serviceId, "paymentId", "token", "payerId");

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}
