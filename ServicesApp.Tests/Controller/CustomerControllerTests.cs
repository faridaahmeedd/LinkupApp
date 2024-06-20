using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using System.Threading.Tasks;
using ServicesApp.Controllers;
using ServicesApp.Dto.User;
using ServicesApp.Interfaces;
using AutoMapper;
using ServicesApp.Core.Models;
using ServicesApp.Helper;
using System;

namespace ServicesApp.Tests.Controller
{
	public class CustomerControllerTests
	{
		private readonly CustomerController _customerController;
		private readonly ICustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public CustomerControllerTests()
		{
			_customerRepository = A.Fake<ICustomerRepository>();
			_mapper = A.Fake<IMapper>();
			_customerController = new CustomerController(_customerRepository, _mapper);
		}

		[Fact]
		public void GetCustomers_WhenCalled_ReturnsOk()
		{
			// Arrange
			var customers = A.Fake<List<Customer>>();
			var mappedCustomers = A.Fake<List<GetCustomerDto>>();
			A.CallTo(() => _customerRepository.GetCustomers()).Returns(customers);
			A.CallTo(() => _mapper.Map<List<GetCustomerDto>>(customers)).Returns(mappedCustomers);

			// Act
			var result = _customerController.GetCustomers();

			// Assert
			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			okResult.Value.Should().BeEquivalentTo(mappedCustomers);
		}

		[Fact]
		public void GetCustomer_CustomerExist_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var customer = A.Fake<Customer>();
			var mappedCustomer = A.Fake<GetCustomerDto>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _customerRepository.GetCustomer(customerId)).Returns(customer);
			A.CallTo(() => _mapper.Map<GetCustomerDto>(customer)).Returns(mappedCustomer);

			// Act
			var result = _customerController.GetCustomer(customerId);

			// Assert
			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			okResult.Value.Should().BeEquivalentTo(mappedCustomer);
		}

		[Fact]
		public void GetCustomer_CustomerDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistingCustomerId";
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			// Act
			var result = _customerController.GetCustomer(customerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.UserNotFound);
		}

		[Fact]
		public void GetCustomer_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			_customerController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = _customerController.GetCustomer(customerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task UpdateProfile_CustomerExist_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var customerUpdate = A.Fake<PostCustomerDto>();
			var mappedCustomer = A.Fake<Customer>();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);
			A.CallTo(() => _mapper.Map<Customer>(customerUpdate)).Returns(mappedCustomer);

			// Act
			var result = await _customerController.UpdateProfile(customerId, customerUpdate);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.SuccessUpdated);
			A.CallTo(() => _customerRepository.UpdateCustomer(mappedCustomer)).MustHaveHappenedOnceExactly();
		}

		[Fact]
		public async Task UpdateProfile_CustomerDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistingCustomerId";
			var customerUpdate = A.Fake<PostCustomerDto>();
			_customerController.ModelState.Clear();                         // Clear any previous ModelState errors
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			// Act
			var result = await _customerController.UpdateProfile(customerId, customerUpdate);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.UserNotFound);
			A.CallTo(() => _customerRepository.UpdateCustomer(A<Customer>._)).MustNotHaveHappened();     // Update method should not be called because there is no customer to update.
		}

		[Fact]
		public async Task UpdateProfile_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			var customerUpdate = A.Fake<PostCustomerDto>();
			_customerController.ModelState.AddModelError("key", "error message"); 

			// Act
			var result = await _customerController.UpdateProfile(customerId, customerUpdate);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			A.CallTo(() => _customerRepository.UpdateCustomer(A<Customer>._)).MustNotHaveHappened();
		}

		[Fact]
		public async Task DeleteCustomer_CustomerExist_ReturnsOk()
		{
			// Arrange
			var customerId = "ExistingCustomerId";
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(true);

			// Act
			var result = await _customerController.DeleteCustomer(customerId);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.SuccessDeleted);
			// Verify that the DeleteCustomer method was called exactly once with the specified customerId
			A.CallTo(() => _customerRepository.DeleteCustomer(customerId)).MustHaveHappenedOnceExactly();
		}

		[Fact]
		public async Task DeleteCustomer_CustomerDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var customerId = "NonExistingCustomerId";
			_customerController.ModelState.Clear();
			A.CallTo(() => _customerRepository.CustomerExist(customerId)).Returns(false);

			// Act
			var result = await _customerController.DeleteCustomer(customerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.UserNotFound);
			A.CallTo(() => _customerRepository.DeleteCustomer(customerId)).MustNotHaveHappened();
		}

		[Fact]
		public async Task DeleteCustomer_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var customerId = "ExistentCustomerId";
			_customerController.ModelState.AddModelError("key", "error message"); 

			// Act
			var result = await _customerController.DeleteCustomer(customerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			A.CallTo(() => _customerRepository.DeleteCustomer(customerId)).MustNotHaveHappened();
		}
	}
}
