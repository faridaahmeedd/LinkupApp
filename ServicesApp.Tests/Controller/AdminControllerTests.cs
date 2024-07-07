using Xunit;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Controllers;
using ServicesApp.Interfaces;
using ServicesApp.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServicesApp.Models;

namespace ServicesApp.Tests.Controller
{
	public class AdminControllerTests
	{
		private readonly IAdminRepository _adminRepository;
		private readonly AdminController _adminController;

		public AdminControllerTests()
		{
			_adminRepository = A.Fake<IAdminRepository>();
			_adminController = new AdminController(_adminRepository);
		}

		[Fact]
		public async Task GetAdmins_WhenCalled_ReturnsOk()
		{
			// Arrange
			var admins = A.Fake<ICollection<AppUser>>();
			A.CallTo(() => _adminRepository.GetAdmins()).Returns(Task.FromResult(admins));

			// Act
			var result = await _adminController.GetAdmins();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task GetAdmins_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			_adminController.ModelState.AddModelError("key", "error message");       // Used to simulate an invalid model state

			// Act
			var result = await _adminController.GetAdmins();

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task GetAdmin_AdminExists_ReturnsOk()
		{
			// Arrange
			var adminId = "Existent Admin";
			var admin = A.Fake<AppUser>();
			A.CallTo(() => _adminRepository.AdminExist(adminId)).Returns(Task.FromResult(true));
			A.CallTo(() => _adminRepository.GetAdmin(adminId)).Returns(Task.FromResult(admin));

			// Act
			var result = await _adminController.GetAdmin(adminId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task GetAdmin_AdminDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var adminId = "Nonexistent Admin";
			A.CallTo(() => _adminRepository.AdminExist(adminId)).Returns(Task.FromResult(false));

			// Act
			var result = await _adminController.GetAdmin(adminId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}


		[Fact]
		public async Task DeleteAdmin_AdminExists_ReturnsOk()
		{
			// Arrange
			var adminId = "Existent Admin";
			A.CallTo(() => _adminRepository.AdminExist(adminId)).Returns(Task.FromResult(true));
			A.CallTo(() => _adminRepository.DeleteAdmin(adminId)).Returns(Task.FromResult(true));

			// Act
			var result = await _adminController.DeleteAdmin(adminId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task DeleteAdmin_AdminDoesNotExist_ReturnsNotFound()
		{
			// Arrange
			var adminId = "Nonexistent Admin";
			A.CallTo(() => _adminRepository.AdminExist(adminId)).Returns(Task.FromResult(false));

			// Act
			var result = await _adminController.DeleteAdmin(adminId);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}
	}
}
