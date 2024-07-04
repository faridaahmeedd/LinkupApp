using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Controllers;
using ServicesApp.Dto.Reviews_Reports;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Tests.Controller
{
	public class ReportControllerTests
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IProviderRepository _providerRepository;
		private readonly IServiceRequestRepository _serviceRequestRepository;
		private readonly IReportRepository _reportRepository;
		private readonly IMapper _mapper;

		public ReportControllerTests()
		{
			_customerRepository = A.Fake<ICustomerRepository>();
			_providerRepository = A.Fake<IProviderRepository>();
			_serviceRequestRepository = A.Fake<IServiceRequestRepository>();
			_reportRepository = A.Fake<IReportRepository>();
			_mapper = A.Fake<IMapper>();
		}

		[Fact]
		public void GetReports_WhenCalled_ReturnsOk()
		{
			// Arrange
			var reports = A.Fake<ICollection<Report>>();
			var mappedReports = A.Fake<List<GetReportDto>>();
			A.CallTo(() => _reportRepository.GetReports()).Returns(reports);
			A.CallTo(() => _mapper.Map<List<GetReportDto>>(reports)).Returns(mappedReports);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetReports();

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetReportById_ReportExist_ReturnsOk()
		{
			// Arrange
			var reportId = 1;  // Existing report Id
			var report = A.Fake<Report>();
			var mappedReport = A.Fake<GetReportDto>();
			A.CallTo(() => _reportRepository.ReportExist(reportId)).Returns(true);
			A.CallTo(() => _reportRepository.GetReport(reportId)).Returns(report);
			A.CallTo(() => _mapper.Map<GetReportDto>(report)).Returns(mappedReport);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetReport(reportId);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetReportById_ReportDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var reportId = 1;  // Existing report Id
			var report = A.Fake<Report>();
			var mappedReport = A.Fake<GetReportDto>();
			A.CallTo(() => _reportRepository.ReportExist(reportId)).Returns(false);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetReport(reportId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetCustomerReports_CustomerExists_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var reports = A.Fake<ICollection<Report>>();
			var reportsList = A.Fake<List<GetReportDto>>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _reportRepository.GetReportsOfCustomer(customerId)).Returns(reports);
			A.CallTo(() => _mapper.Map<List<GetReportDto>>(reports)).Returns(reportsList);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetCustomerReports(customerId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetCustomerReports_CustomerDoesNotExists_ReturnsOk()
		{
			// Arrange
			var customerId = "NonExistentCustomerId";
			var reports = A.Fake<ICollection<Report>>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetCustomerReports(customerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetCustomerReports_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);
			_reportController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = _reportController.GetCustomerReports(customerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public void GetProviderReports_ProviderExists_ReturnsOk()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var reports = A.Fake<ICollection<Report>>();
			var reportsList = A.Fake<List<GetReportDto>>();
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
			A.CallTo(() => _reportRepository.GetReportsOfProvider(providerId)).Returns(reports);
			A.CallTo(() => _mapper.Map<List<GetReportDto>>(reports)).Returns(reportsList);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetProviderReports(providerId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public void GetProviderReports_ProviderDoesNotExists_ReturnsOk()
		{
			// Arrange
			var providerId = "NonExistentProviderId";
			var reports = A.Fake<ICollection<Report>>();
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.GetProviderReports(providerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public void GetProviderReports_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);
			_reportController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = _reportController.GetProviderReports(providerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task CreateCustomerReport_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var requestId = 999; // Non-existing service ID
			var reportDto = A.Fake<PostReportDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(false);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.CreateCustomerReport(reportDto, requestId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
		}

		[Fact]
		public async Task CreateProviderReport_ServiceNotFound_ReturnsNotFound()
		{
			// Arrange
			var requestId = 999; // Non-existing service ID
			var reportDto = A.Fake<PostReportDto>();
			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(false);
			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result =  _reportController.CreateProviderReport(reportDto, requestId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
		}

		[Fact]
		public async Task CreateCustomerReview_ValidRequest_ReturnsOk()
		{
			// Arrange
			var requestId = 1;
			var reportDto = A.Fake<PostReportDto>();
			var mappedReport = A.Fake<Report>();
			var request = A.Fake<ServiceRequest>();

			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			//A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(true);
			//A.CallTo(() => _reportRepository.IsProviderAlreadyReported(requestId)).Returns(false);
			A.CallTo(() => _mapper.Map<Report>(reportDto)).Returns(mappedReport);
			A.CallTo(() => _serviceRequestRepository.GetService(requestId)).Returns(request);

			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.CreateCustomerReport(reportDto, requestId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
		}

		[Fact]
		public async Task CreateProviderReport_ValidRequest_ReturnsOk()
		{
			// Arrange
			var requestId = 1;
			var reportDto = A.Fake<PostReportDto>();
			var mappedReport = A.Fake<Report>();
			var request = A.Fake<ServiceRequest>();

			A.CallTo(() => _serviceRequestRepository.ServiceExist(requestId)).Returns(true);
			//A.CallTo(() => _serviceRequestRepository.CheckRequestCompleted(requestId)).Returns(true);
			//A.CallTo(() => _reportRepository.IsCustomerAlreadyReported(requestId)).Returns(false);
			A.CallTo(() => _mapper.Map<Report>(reportDto)).Returns(mappedReport);
			A.CallTo(() => _serviceRequestRepository.GetService(requestId)).Returns(request);

			var _reportController = new ReportController(_reportRepository, _mapper, _providerRepository, _customerRepository, _serviceRequestRepository);

			// Act
			var result = _reportController.CreateProviderReport(reportDto, requestId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(200, okResult.StatusCode);
		}

		//[Fact]
		//public async Task CreateCustomerReport_ServiceAlreadyReported_ReturnsBadRequest()
		//{

		//}

		//[Fact]
		//public async Task CreateProviderReport_ServiceAlreadyReported_ReturnsBadRequest()
		//{

		//}
	}
}
