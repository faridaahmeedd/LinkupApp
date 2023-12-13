using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.Dto.Authentication;
using ServicesApp.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthRepository
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IConfiguration _config;
	private readonly RoleManager<IdentityRole> _roleManager;

	public AuthRepository(
		UserManager<AppUser> userManager,
		IConfiguration config,
		RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_config = config;
		_roleManager = roleManager;
	}

	public async Task<IdentityResult> RegisterUser(RegistrationDto registerDto, string role)
	{
		// Check if user already exists
		var appUser = await _userManager.FindByEmailAsync(registerDto.Email);
		if (appUser != null)
		{
			return IdentityResult.Failed(new IdentityError { Description = "Account Already Exists" });
		}

		// Add the user to DB
		var user = new AppUser
		{
			Email = registerDto.Email,
			SecurityStamp = Guid.NewGuid().ToString(),
			UserName = registerDto.Email,
		};

		// Check if the role exists
		if (await _roleManager.RoleExistsAsync(role))
		{
			var result = await _userManager.CreateAsync(user, registerDto.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, role);
				return result;
			}
			return IdentityResult.Failed(new IdentityError { Description = "Error while creating user" });
		}

		return IdentityResult.Failed(new IdentityError { Description = "Role Doesn't Exist" });
	}

	public async Task<(string Token, DateTime Expiration)> LoginUser(LoginDto loginDto)
	{
		var appUser = await _userManager.FindByEmailAsync(loginDto.Email);
		if (appUser != null && await _userManager.CheckPasswordAsync(appUser, loginDto.Password))
		{
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, appUser.Email),
				new Claim(ClaimTypes.NameIdentifier, appUser.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			var roles = await _userManager.GetRolesAsync(appUser);
			authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var expiration = DateTime.Now.AddHours(3);

			var jwtToken = new JwtSecurityToken(
				issuer: _config["JWT:ValidIssuer"],
				audience: _config["JWT:ValidAudience"],
				claims: authClaims,
				expires: expiration,
				signingCredentials: signingCredentials
			);

			var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

			return (token, expiration);
		}

		return (null, DateTime.MinValue);
	}
}
