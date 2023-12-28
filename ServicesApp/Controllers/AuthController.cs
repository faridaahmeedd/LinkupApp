using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.Authentication;
using ServicesApp.APIs;
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
			var appUser = await _authRepository.CheckUser(registerDto.Email);
			if (appUser != null)
			{
				return BadRequest(ApiResponse.UserAlreadyExist);
			}
			if (await _authRepository.CheckRole(role))
			{
                

                var res = await _authRepository.CreateUser(registerDto, role);
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

                return BadRequest(res.Errors);   //// ----->
			}
			return BadRequest(ApiResponse.RoleDoesNotExist);
		}
		return BadRequest(ApiResponse.NotValid);
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
                    statusMsg = "success",
                    message = "Logged in Successfully.",
                    Token = token,
					Expiration = expiration,
				});
			}
		}
		return Unauthorized(ApiResponse.UnAutharized);
	}

	[HttpPost("ForgetPassword")]
	public  async Task<IActionResult> ForgetPassword(string mail)
	{
        var resetCode = await _authRepository.ForgetPassword(mail);

        if (resetCode != string.Empty)
		{
			return Ok(new {
                code=  resetCode,
				statusMsg = "success",
                message = "Reset Code Sent Successfully.",

            }
            );
		}
		return BadRequest(ApiResponse.CanNotSentMail);
	}

    [HttpPut("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] RegistrationDto registrationDto)
    {
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
