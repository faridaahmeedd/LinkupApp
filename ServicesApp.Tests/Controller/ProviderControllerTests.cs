using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServicesApp.Controllers;
using ServicesApp.Dto.User;
using ServicesApp.Interfaces;
using AutoMapper;
using ServicesApp.Core.Models;
using ServicesApp.Helper;
using System;
using ServicesApp.Models;

namespace ServicesApp.Tests.Controller
{
	public class ProviderControllerTests
	{
		private readonly ProviderController _providerController;
		private readonly IProviderRepository _providerRepository;
		private readonly IMapper _mapper;

		public ProviderControllerTests()
		{
			_providerRepository = A.Fake<IProviderRepository>();
			_mapper = A.Fake<IMapper>();
			_providerController = new ProviderController(_providerRepository, _mapper);
		}

		[Fact]
		public void GetProviders_WhenCalled_ReturnsOk()
		{
			// Arrange
			var providers = A.Fake<List<Provider>>();
			var mappedProviders = A.Fake<List<GetProviderDto>>();
			A.CallTo(() => _providerRepository.GetProviders()).Returns(providers);
			A.CallTo(() => _mapper.Map<List<GetProviderDto>>(providers)).Returns(mappedProviders);

			// Act
			var result = _providerController.GetProviders();

			// Assert
			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			okResult.Value.Should().BeEquivalentTo(mappedProviders);
		}

		[Fact]
		public void GetProvider_ProviderExists_ReturnsOk()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var provider = A.Fake<Provider>();
			var mappedProvider = A.Fake<GetProviderDto>();
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
			A.CallTo(() => _providerRepository.GetProvider(providerId)).Returns(provider);
			A.CallTo(() => _mapper.Map<GetProviderDto>(provider)).Returns(mappedProvider);

			// Act
			var result = _providerController.GetProvider(providerId);

			// Assert
			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			okResult.Value.Should().BeEquivalentTo(mappedProvider);
		}

		[Fact]
		public void GetProvider_ProviderDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var providerId = "NonExistingProviderId";
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

			// Act
			var result = _providerController.GetProvider(providerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.UserNotFound);
		}

		[Fact]
		public void GetProvider_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			_providerController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = _providerController.GetProvider(providerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task UpdateProfile_ProviderExists_ReturnsOk()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var providerUpdate = A.Fake<PostProviderDto>();
			var mappedProvider = A.Fake<Provider>();
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);
			A.CallTo(() => _mapper.Map<Provider>(providerUpdate)).Returns(mappedProvider);

			// Act
			var result = await _providerController.UpdateProfile(providerId, providerUpdate);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.SuccessUpdated);
			A.CallTo(() => _providerRepository.UpdateProvider(mappedProvider)).MustHaveHappenedOnceExactly();
		}

		[Fact]
		public async Task UpdateProfile_ProviderDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var providerId = "NonExistingProviderId";
			var providerUpdate = A.Fake<PostProviderDto>();
			_providerController.ModelState.Clear(); // Clear any previous ModelState errors
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

			// Act
			var result = await _providerController.UpdateProfile(providerId, providerUpdate);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.UserNotFound);
			A.CallTo(() => _providerRepository.UpdateProvider(A<Provider>._)).MustNotHaveHappened();
		}

		[Fact]
		public async Task UpdateProfile_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var providerId = "ExistentProviderId";
			var providerUpdate = A.Fake<PostProviderDto>();
			_providerController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = await _providerController.UpdateProfile(providerId, providerUpdate);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			A.CallTo(() => _providerRepository.UpdateProvider(A<Provider>._)).MustNotHaveHappened();
		}

		[Fact]
		public async Task DeleteProvider_ProviderExists_ReturnsOk()
		{
			// Arrange
			var providerId = "ExistingProviderId";
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(true);

			// Act
			var result = await _providerController.DeleteProvider(providerId);

			// Assert
			result.Should().BeOfType<OkObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.SuccessDeleted);
			A.CallTo(() => _providerRepository.DeleteProvider(providerId)).MustHaveHappenedOnceExactly();
		}

		[Fact]
		public async Task DeleteProvider_ProviderDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var providerId = "NonExistingProviderId";
			_providerController.ModelState.Clear(); // Clear any previous ModelState errors
			A.CallTo(() => _providerRepository.ProviderExist(providerId)).Returns(false);

			// Act
			var result = await _providerController.DeleteProvider(providerId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>()
				  .Which.Value.Should().Be(ApiResponses.UserNotFound);
			A.CallTo(() => _providerRepository.DeleteProvider(providerId)).MustNotHaveHappened();
		}

		[Fact]
		public async Task DeleteProvider_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var providerId = "ExistingProviderId";
			_providerController.ModelState.AddModelError("key", "error message");       // Simulate an invalid model state

			// Act
			var result = await _providerController.DeleteProvider(providerId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			A.CallTo(() => _providerRepository.DeleteProvider(providerId)).MustNotHaveHappened();
		}
	}
}
