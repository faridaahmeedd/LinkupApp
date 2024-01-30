using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Authentication;
using ServicesApp.APIs;
using ServicesApp.Interfaces;
using ServicesApp.Core.Models;
using ServicesApp.Repository;

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
		if (ModelState.IsValid)
		{
			var appUser = await _authRepository.CheckUser(registerDto.Email);
			if (appUser != null)
			{
				return BadRequest(ApiResponse.UserAlreadyExist);
			}
			if (await _authRepository.CheckRole(Role))
			{
                var res = await _authRepository.CreateUser(registerDto, Role);
				if(res.Succeeded)
				{
                    appUser = await _authRepository.CheckUser(registerDto.Email);
                    return Ok(new
					{
                        statusMsg = "success",
                        message = "User Created Successfully.",
                        userId = appUser.Id
                    }) ;
                }
                return BadRequest(ApiResponse.PasswordInValid);
            }
			return BadRequest(ApiResponse.RoleDoesNotExist);
		}
		return BadRequest(ApiResponse.NotValid);
	}

	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
	{
		if (ModelState.IsValid)
		{
			var (token, expiration , role) = await _authRepository.LoginUser(loginDto);

			if (token != null)
			{
				return Ok(new
				{
                    statusMsg = "success",
                    message = "Logged in Successfully.",
                    Token = token,
					Expiration = expiration,
					Role = role
				});
			}
		}
		return Unauthorized(ApiResponse.Unuthorized);
	}

	[HttpPost("ForgetPassword/{Mail}")]
	public  async Task<IActionResult> ForgetPassword(string Mail)
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
			return Ok(new {
                code=  resetCode,
				statusMsg = "success",
                message = "Reset Code Sent Successfully.",
            });
		}
		return BadRequest(ApiResponse.CanNotSentMail);
	}

    [HttpPut("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] RegistrationDto registrationDto)
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
}