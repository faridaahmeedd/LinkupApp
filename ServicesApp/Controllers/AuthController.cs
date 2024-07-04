﻿using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Authentication;
using ServicesApp.Interfaces;
using ServicesApp.Helper;
using Google.Apis.Auth;
using Azure.Core;
using ServicesApp.Dto.User;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthRepository _authRepository;

	public AuthController(IAuthRepository authenticationRepository)
	{
		_authRepository = authenticationRepository;
	}

	[HttpPost("RegisterUser/{Role}")]
	public async Task<IActionResult> RegisterUser([FromBody] RegistrationDto registerDto, string Role)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			if (!await _authRepository.CheckRole(Role))
			{
				return BadRequest(ApiResponses.RoleDoesNotExist);
			}
			var appUser = await _authRepository.CheckUser(registerDto.Email);
			if (appUser != null)
			{
				return BadRequest(ApiResponses.UserAlreadyExist);
			}
			var res = await _authRepository.CreateUser(registerDto, Role);
			if (res.Succeeded)
			{
				appUser = await _authRepository.CheckUser(registerDto.Email);
				return Ok(new
				{
					statusMsg = "success",
					message = "User Created Successfully.",
					userId = appUser.Id
				});
			}
			if(!_authRepository.CheckValidPassword(res.Errors))
			{
				return BadRequest(ApiResponses.InvalidPass);
			}
			return BadRequest(ApiResponses.NotValid);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPost("RegisterAdmin")]
	// [Authorize(Roles = "SuperAdmin")]
	public async Task<IActionResult> RegisterAdmin([FromBody] RegistrationDto registerDto)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			var appUser = await _authRepository.CheckAdmin(registerDto.Email);
			if (appUser != null)
			{
				return BadRequest(ApiResponses.AdminAlreadyExist);
			}
			var res = await _authRepository.CreateAdmin(registerDto);
			if (res.Succeeded)
			{
				appUser = await _authRepository.CheckAdmin(registerDto.Email);
				return Ok(new
				{
					statusMsg = "success",
					message = "Admin Created Successfully.",
					userId = appUser.Id
				});
			}
			if (!_authRepository.CheckValidPassword(res.Errors))
			{
				return BadRequest(ApiResponses.InvalidPass);
			}
			return BadRequest(ApiResponses.NotValid);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			var appUser = await _authRepository.CheckUser(loginDto.Email);
			if (appUser != null && !appUser.EmailConfirmed)
			{
				return Unauthorized(ApiResponses.EmailNotVerified);
			}
			var (token, expiration) = await _authRepository.LoginUser(loginDto);
			if (token != null)
			{
				return Ok(new
				{
					statusMsg = "success",
					message = "Logged in Successfully.",
					Token = token,
					Expiration = expiration,
				});
			}
			return Unauthorized(ApiResponses.Unauthorized);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPost("GoogleLogin/{Token}")]
	public async Task<IActionResult> GoogleLogin(string Token)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			var payload = await _authRepository.VerifyGoogleToken(Token);
			if (payload == null)
			{
				return Unauthorized(ApiResponses.Unauthorized);
			}
			var (token, expiration) = await _authRepository.LoginGoogleUser(payload);
			if (token != null)
			{
				return Ok(new
				{
					statusMsg = "success",
					message = "Logged in Successfully.",
					Token = token,
					Expiration = expiration,
				});
			}
			return Unauthorized(ApiResponses.Unauthorized);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPost("ForgetPassword/{Mail}")]
	public async Task<IActionResult> ForgetPassword(string Mail)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			var user = await _authRepository.CheckUser(Mail);
			if (user == null)
			{
				return NotFound(ApiResponses.UserNotFound);
			}
			var resetCode = await _authRepository.ForgetPassword(Mail);

			if (resetCode != string.Empty)
			{
				return Ok(new
				{
					code = resetCode,
					statusMsg = "success",
					message = "Reset Code Sent Successfully.",
				});
			}
			return BadRequest(ApiResponses.CanNotSentMail);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPut("ResetPassword")]
	public async Task<IActionResult> ResetPassword([FromBody] RegistrationDto registrationDto)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			var user = await _authRepository.CheckUser(registrationDto.Email);
			if (user == null)
			{
				return NotFound(ApiResponses.UserNotFound);
			}
			if (registrationDto.Password == registrationDto.ConfirmPassword)
			{
				if (await _authRepository.ResetPassword(registrationDto.Email, registrationDto.Password))
				{
					return Ok(ApiResponses.PassChanged);
				}
			}
			return BadRequest(ApiResponses.CanNotChangePass);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPut("VerifyOtp/{Email}/{Otp}")]
	public async Task<IActionResult> VerifyOtp(string Email, string Otp)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var user = await _authRepository.CheckUser(Email);
			if (user == null)
			{
				return NotFound(ApiResponses.UserNotFound);
			}

			var isValidOtp = await _authRepository.VerifyOtp(Email, Otp);
			if (!isValidOtp)
			{
				return Unauthorized(ApiResponses.InvalidOtp);
			}
			return Ok(ApiResponses.OtpVerified);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPut("Deactivate/{UserId}")]
	public async Task<IActionResult> DeactivateUser(string UserId, [FromBody] DeactivationDto DeactivateDto)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponses.NotValid);
			}
			var user = await _authRepository.CheckUserById(UserId);
			if (user == null)
			{
				return NotFound(ApiResponses.UserNotFound);
			}
			if (await _authRepository.DeactivateUser(UserId, DeactivateDto.Reason))
			{
				return Ok(ApiResponses.UserDeactivated);
			}
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}
}