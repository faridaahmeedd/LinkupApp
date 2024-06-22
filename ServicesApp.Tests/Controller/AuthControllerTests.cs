using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServicesApp.Controllers;
using ServicesApp.Dto.Authentication;
using ServicesApp.Helper;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Tests.Controller
{
	public class AuthControllerTests
	{
		private readonly IAuthRepository _authRepository;
		private readonly AuthController _authController;

		public AuthControllerTests()
		{
			_authRepository = A.Fake<IAuthRepository>();
			_authController = new AuthController(_authRepository);
		}

		[Fact]
		public async Task RegisterUser_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var role = "ExistingRole";
			_authController.ModelState.AddModelError("Error", "ModelState is invalid");

			// Act
			var result = await _authController.RegisterUser(registrationDto, role);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.NotValid, actionResult.Value);
		}

		[Fact]
		public async Task RegisterUser_RoleNotFound_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var role = "NonExistingRole";
			A.CallTo(() => _authRepository.CheckRole(role)).Returns(Task.FromResult(false));

			// Act
			var result = await _authController.RegisterUser(registrationDto, role);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterUser_UserAlreadyExists_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var appUser = A.Fake<AppUser>();
			var role = "ExistingRole";
			A.CallTo(() => _authRepository.CheckRole(role)).Returns(Task.FromResult(true));
			A.CallTo(() => _authRepository.CheckUser(registrationDto.Email)).Returns(appUser);

			// Act
			var result = await _authController.RegisterUser(registrationDto, role);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterUser_UserCreatedSuccessfully_ReturnsOk()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var appUser = A.Fake<AppUser>();
			var role = "Customer";
			A.CallTo(() => _authRepository.CheckRole(role)).Returns(Task.FromResult(true));
			A.CallTo(() => _authRepository.CheckUser(registrationDto.Email))
				.ReturnsNextFromSequence(Task.FromResult<AppUser?>(null), Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.CreateUser(registrationDto, role)).Returns(IdentityResult.Success);

			// Act
			var result = await _authController.RegisterUser(registrationDto, role);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
			Assert.Equal("User Created Successfully.", responseObject["message"]?.ToString());
			Assert.Equal(appUser.Id, responseObject["userId"]?.ToString());
		}

		[Fact]
		public async Task RegisterUser_InvalidPassword_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var appUser = A.Fake<AppUser>();
			var identityResult = A.Fake<IdentityResult>();
			var role = "Customer";
			A.CallTo(() => _authRepository.CheckRole(role)).Returns(Task.FromResult(true));
			A.CallTo(() => _authRepository.CheckUser(registrationDto.Email))
				.ReturnsNextFromSequence(Task.FromResult<AppUser?>(null), Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.CreateUser(registrationDto, role)).Returns(identityResult);
			A.CallTo(() => _authRepository.CheckValidPassword(identityResult.Errors)).Returns(false);

			// Act
			var result = await _authController.RegisterUser(registrationDto, role);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.InvalidPass, actionResult.Value);
		}

		[Fact]
		public async Task RegisterAdmin_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			_authController.ModelState.AddModelError("Error", "ModelState is invalid");

			// Act
			var result = await _authController.RegisterAdmin(registrationDto);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.NotValid, actionResult.Value);
		}

		[Fact]
		public async Task RegisterAdmin_AdminAlreadyExists_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckAdmin(registrationDto.Email)).Returns(appUser);

			// Act
			var result = await _authController.RegisterAdmin(registrationDto);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task RegisterAdmin_AdminCreatedSuccessfully_ReturnsOk()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckAdmin(registrationDto.Email))
				.ReturnsNextFromSequence(Task.FromResult<AppUser?>(null), Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.CreateAdmin(registrationDto)).Returns(IdentityResult.Success);

			// Act
			var result = await _authController.RegisterAdmin(registrationDto);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
			Assert.Equal("Admin Created Successfully.", responseObject["message"]?.ToString());
			Assert.Equal(appUser.Id, responseObject["userId"]?.ToString());
		}

		[Fact]
		public async Task RegisterAdmin_InvalidPassword_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var userId = A.Fake<AppUser>();
			var identityResult = A.Fake<IdentityResult>();
			A.CallTo(() => _authRepository.CheckAdmin(registrationDto.Email))
				.ReturnsNextFromSequence(Task.FromResult<AppUser?>(null), Task.FromResult<AppUser?>(userId));
			A.CallTo(() => _authRepository.CreateAdmin(registrationDto)).Returns(identityResult);
			A.CallTo(() => _authRepository.CheckValidPassword(identityResult.Errors)).Returns(false);

			// Act
			var result = await _authController.RegisterAdmin(registrationDto);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.InvalidPass, actionResult.Value);
		}

		[Fact]
		public async Task Login_ValidCredentials_ReturnsOk()
		{
			// Arrange
			var loginDto = A.Fake<LoginDto>();
			var appUser = A.Fake<AppUser>();
			appUser.Id = Guid.NewGuid().ToString();
			var token = "dummy_token";
			var expiration = DateTime.UtcNow.AddDays(1);
			A.CallTo(() => _authRepository.LoginUser(loginDto)).Returns(Task.FromResult((token, expiration)));

			// Act
			var result = await _authController.Login(loginDto);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
			Assert.Equal(token, responseObject["Token"]?.ToString());
			Assert.Equal(expiration.ToString(), responseObject["Expiration"]?.ToString());
		}

		[Fact]
		public async Task Login_InvalidCredentials_ReturnsBadRequest()
		{
			// Arrange
			var loginDto = A.Fake<LoginDto>();
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.LoginUser(loginDto)).Returns(Task.FromResult<(string, DateTime)>((null, default)));

			// Act
			var result = await _authController.Login(loginDto);

			// Assert
			var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
		}

		[Fact]
		public async Task ForgetPassword_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var email = "mail@example.com";
			_authController.ModelState.AddModelError("Error", "ModelState is invalid");

			// Act
			var result = await _authController.ForgetPassword(email);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.NotValid, actionResult.Value);
		}

		[Fact]
		public async Task ForgetPassword_UserNotFound_ReturnsNotFound()
		{
			// Arrange
			var email = "mail@example.com";
			A.CallTo(() => _authRepository.CheckUser(email)).Returns(Task.FromResult<AppUser?>(null));

			// Act
			var result = await _authController.ForgetPassword(email);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task ForgetPassword_ResetCodeSent_ReturnsOk()
		{
			// Arrange
			var email = "mail@example.com";
			var resetCode = "dummyCode";
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckUser(email)).Returns(Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.ForgetPassword(email)).Returns(resetCode);

			// Act
			var result = await _authController.ForgetPassword(email);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal("success", responseObject["statusMsg"]?.ToString());
			Assert.Equal(resetCode, responseObject["code"]?.ToString());
		}

		[Fact]
		public async Task ForgetPassword_ResetCodeNotSent_ReturnsBadRequest()
		{
			// Arrange
			var email = "mail@example.com";
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckUser(email)).Returns(Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.ForgetPassword(email)).Returns(string.Empty);


			// Act
			var result = await _authController.ForgetPassword(email);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal("fail", responseObject["statusMsg"]?.ToString());
		}

		[Fact]
		public async Task ResetPassword_ModelStateIsInvalid_ReturnsBadRequest()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			_authController.ModelState.AddModelError("Error", "ModelState is invalid");

			// Act
			var result = await _authController.ResetPassword(registrationDto);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(ApiResponses.NotValid, actionResult.Value);
		}

		[Fact]
		public async Task ResetPassword_UserNotFound_ReturnsNotFound()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			A.CallTo(() => _authRepository.CheckUser(registrationDto.Email)).Returns(Task.FromResult<AppUser?>(null));

			// Act
			var result = await _authController.ResetPassword(registrationDto);

			// Assert
			var actionResult = Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task ResetPassword_PasswordChanged_ReturnsOk()
		{
			// Arrange
			var registrationDto = A.Fake<RegistrationDto>();
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckUser(registrationDto.Email)).Returns(Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.ResetPassword(registrationDto.Email, registrationDto.Password)).Returns(Task.FromResult(true));

			// Act
			var result = await _authController.ResetPassword(registrationDto);

			// Assert
			var actionResult = Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task DeactivateUser_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var appUser = A.Fake<AppUser>();
			_authController.ModelState.AddModelError("Error", "ModelState is invalid");

			// Act
			var result = await _authController.DeactivateUser(appUser.Id);

			// Assert
			var actionResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task DeactivateUser_UserNotFound_ReturnsNotFound()
		{
			// Arrange
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckUserById(appUser.Id)).Returns(Task.FromResult<AppUser?>(null));

			// Act
			var result = await _authController.DeactivateUser(appUser.Id);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task DeactivateUser_UserDeactivated_ReturnsOk()
		{
			// Arrange
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckUserById(appUser.Id)).Returns(Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.DeactivateUser(appUser.Id)).Returns(Task.FromResult(true));

			// Act
			var result = await _authController.DeactivateUser(appUser.Id);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task DeactivateUser_DeactivationFails_Returns500()
		{
			// Arrange
			var appUser = A.Fake<AppUser>();
			A.CallTo(() => _authRepository.CheckUserById(appUser.Id)).Returns(Task.FromResult<AppUser?>(appUser));
			A.CallTo(() => _authRepository.DeactivateUser(appUser.Id)).Returns(Task.FromResult(false));

			// Act
			var result = await _authController.DeactivateUser(appUser.Id);

			// Assert
			var actionResult = Assert.IsType<ObjectResult>(result);
			var responseObject = JObject.FromObject(actionResult.Value);
			Assert.Equal(500, actionResult.StatusCode);
			Assert.Equal("fail", responseObject["statusMsg"]?.ToString());
		}
	}
}
