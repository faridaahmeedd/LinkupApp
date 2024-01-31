﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Authentication;
using ServicesApp.APIs;
using ServicesApp.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthRepository _authRepository;

	public AuthController(IAuthRepository authenticationRepository)
	{
		_authRepository = authenticationRepository;
	}

	[HttpPost("Register/{Role}")]
	public async Task<IActionResult> Register([FromBody] RegistrationDto registerDto, string Role)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var appUser = await _authRepository.CheckUser(registerDto.Email);
			if (appUser != null)
			{
				return BadRequest(ApiResponse.UserAlreadyExist);
			}
			if (await _authRepository.CheckRole(Role))
			{
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
				foreach (var error in res.Errors)
				{
					if (error.Code.StartsWith("Password"))
					{
						return BadRequest(ApiResponse.InvalidEmailOrPass);
					}
				}
			}
			return BadRequest(ApiResponse.RoleDoesNotExist);
		}
		catch
		{
			return StatusCode(500, ApiResponse.SomethingWrong);
		}
	}

	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
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
			return Unauthorized(ApiResponse.Unuthorized);
		}
		catch
		{
			return StatusCode(500, ApiResponse.SomethingWrong);
		}
	}

	[HttpPost("ForgetPassword/{Mail}")]
	public  async Task<IActionResult> ForgetPassword(string Mail)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var user = await _authRepository.CheckUser(Mail);
			if (user == null)
			{
				return NotFound(ApiResponse.UserNotFound);
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
			return BadRequest(ApiResponse.CanNotSentMail);
		}
		catch
		{
			return StatusCode(500, ApiResponse.SomethingWrong);
		}
	}

    [HttpPut("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] RegistrationDto registrationDto)
    {
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var user = await _authRepository.CheckUser(registrationDto.Email);
			if (user == null)
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			if (registrationDto.Password == registrationDto.ConfirmPassword)
			{
				Console.Write(registrationDto.Password);

				var resetPassword = await _authRepository.ResetPassword(registrationDto.Email, registrationDto.Password);

				if (resetPassword)
				{
					return Ok(ApiResponse.PassChanged);
				}
			}
			return BadRequest(ApiResponse.CanNotChangePass);
		}
		catch
		{
			return StatusCode(500, ApiResponse.SomethingWrong);
		}
	}
}