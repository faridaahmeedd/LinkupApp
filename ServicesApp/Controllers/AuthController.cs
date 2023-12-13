using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Authentication;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly AuthRepository _authRepository;

	public AuthController(AuthRepository authenticationRepository)
	{
		_authRepository = authenticationRepository;
	}

	[HttpPost("Register")]
	public async Task<IActionResult> Register([FromBody] RegistrationDto registerDto, string role)
	{
		if (ModelState.IsValid)
		{
			var result = await _authRepository.RegisterUser(registerDto, role);

			if (result.Succeeded)
			{
				return Ok("Registration Successful");
			}

			return BadRequest(result.Errors);
		}

		return BadRequest();
	}

	[HttpPost("Login")]
	public async Task<IActionResult> Login(LoginDto loginDto)
	{
		if (ModelState.IsValid)
		{
			var (token, expiration) = await _authRepository.LoginUser(loginDto);

			if (token != null)
			{
				return Ok(new
				{
					Token = token,
					Expiration = expiration,
				});
			}
		}

		return Unauthorized();
	}
}
