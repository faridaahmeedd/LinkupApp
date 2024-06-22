using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Controllers;
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
	public class PayMobControllerTests
	{
		private readonly IPayMobRepository _payMobRepository;
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly PayMobController _payMobController;

		public PayMobControllerTests()
		{
			_payMobRepository = A.Fake<IPayMobRepository>();
			_serviceRepository = A.Fake<IServiceRequestRepository>();
			_payMobController = new PayMobController(_payMobRepository, _serviceRepository);
		}

		[Fact]
		public async Task PayService_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = await _payMobController.PayService(serviceId);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, actionResult.Value);
		}

		[Fact]
		public async Task PayService_AcceptedOfferNotFound_ReturnsNotFound()
		{
			// Arrange
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(null);

			// Act
			var result = await _payMobController.PayService(serviceId);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.OfferNotFound, actionResult.Value);
		}

		[Fact]
		public async Task PayService_ServiceAlreadyPaid_ReturnsBadRequest()
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
			var result = await _payMobController.PayService(serviceId);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.PaidAlready, actionResult.Value);
		}

		[Fact]
		public async Task PayService_PaymentSuccessful_ReturnsOk()
		{
			// Arrange
			var serviceId = 1;
			var paymentLink = "http://paymentlink.com";
			var request = A.Fake<ServiceRequest>();
			var offer = A.Fake<ServiceOffer>();
			request.PaymentStatus = "Pending";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(offer);
			A.CallTo(() => _serviceRepository.GetService(serviceId)).Returns(request);
			A.CallTo(() => _payMobRepository.CardPayment(serviceId)).Returns(Task.FromResult(paymentLink));

			// Act
			var result = await _payMobController.PayService(serviceId);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(paymentLink, actionResult.Value);
		}

		[Fact]
		public async Task PayService_PaymentFailed_ReturnsBadRequest()
		{
			// Arrange
			var serviceId = 1;
			var request = A.Fake<ServiceRequest>();
			var offer = A.Fake<ServiceOffer>();
			request.PaymentStatus = "Pending";
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _serviceRepository.GetAcceptedOffer(serviceId)).Returns(offer);
			A.CallTo(() => _serviceRepository.GetService(serviceId)).Returns(request);
			A.CallTo(() => _payMobRepository.CardPayment(serviceId)).Returns(Task.FromResult<string>(null));

			// Act
			var result = await _payMobController.PayService(serviceId);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.PaymentError, actionResult.Value);
		}

		[Fact]
		public async Task CaptureTransaction_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var transactionId = 1;
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(false);

			// Act
			var result = await _payMobController.CaptureTransaction(transactionId, serviceId);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(ApiResponses.RequestNotFound, actionResult.Value);
		}

		[Fact]
		public async Task CaptureTransaction_CaptureSuccessful_ReturnsOk()
		{
			// Arrange
			var transactionId = 1;
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _payMobRepository.Capture(transactionId, serviceId)).Returns(Task.FromResult(true));


			// Act
			var result = await _payMobController.CaptureTransaction(transactionId, serviceId);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(ApiResponses.CaptureSuccess, actionResult.Value);
		}

		[Fact]
		public async Task CaptureTransaction_CaptureFailed_ReturnsBadRequest()
		{
			// Arrange
			var transactionId = 1;
			var serviceId = 1;
			A.CallTo(() => _serviceRepository.ServiceExist(serviceId)).Returns(true);
			A.CallTo(() => _payMobRepository.Capture(transactionId, serviceId)).Returns(Task.FromResult(false));

			// Act
			var result = await _payMobController.CaptureTransaction(transactionId, serviceId);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.CannotCapture, actionResult.Value);
		}
	}
}
