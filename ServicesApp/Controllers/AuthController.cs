using Microsoft.AspNetCore.Identity;
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
			var appUser = await _authRepository.CheckUser(registerDto.Email);
			if (appUser != null)
			{
				return BadRequest("User already exists");
			}
			if (await _authRepository.CheckRole(role))
			{
                Console.WriteLine("role controller  ");

                var res = await _authRepository.CreateUser(registerDto, role);
				if(res.Succeeded)
				{
                    Console.WriteLine("cteate user controller ");

                    appUser = await _authRepository.CheckUser(registerDto.Email);
                    return Ok(appUser.Id);
                }


                //List<IdentityError> errorList = res.Errors. ToList();
                //var errors = string.Join(", ", errorList.Select(e => e.Description));
                return BadRequest(res.Errors);
			}
			return BadRequest("Role doesn't exist");
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

	[HttpPost("ForgetPassword")]
	public  async Task<IActionResult> ForgetPassword(string mail)
	{
        var resetCode = await _authRepository.ForgetPassword(mail);

        if (resetCode != string.Empty)
		{
			return Ok(resetCode);
		}
		return BadRequest("Can not send mail");
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
                return Ok("Password Changed Successfully.");

            }
        }
        return BadRequest("Can not Change Password.");
    }
}
