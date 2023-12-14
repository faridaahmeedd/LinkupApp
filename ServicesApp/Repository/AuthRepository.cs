using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.Core.Models;
using ServicesApp.Dto.Authentication;
using ServicesApp.Interfaces;
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
	private readonly IMapper _mapper;
	private readonly ICustomerRepository _customerRepository;
	private readonly IProviderRepository _providerRepository;

	public AuthRepository(
		UserManager<AppUser> userManager,
		IConfiguration config,
		RoleManager<IdentityRole> roleManager,
		IMapper mapper,
		ICustomerRepository customerRepository,
		IProviderRepository providerRepository)
	{
		_userManager = userManager;
		_config = config;
		_roleManager = roleManager;
		_mapper = mapper;
		_customerRepository = customerRepository;
		_providerRepository = providerRepository;
	}

	public async Task<AppUser?> CheckUser(string email)
	{
		var appUser = await _userManager.FindByEmailAsync(email);
		if (appUser != null)
		{
			return appUser;
		}
		return null;
	}

	public async Task<bool> CheckRole(string role)
	{
		if (await _roleManager.RoleExistsAsync(role))
		{
			return true;
		}
		return false;
	}

	public async Task<bool> CreateUser(RegistrationDto registerDto, string role)
	{
		if(role == "Customer")
		{
			var userMap = _mapper.Map<Customer>(registerDto);
			userMap.Email = registerDto.Email;
			userMap.SecurityStamp = Guid.NewGuid().ToString();
			userMap.UserName = registerDto.Email;
			if (_customerRepository.CreateCustomer(userMap))
			{
				await _userManager.AddPasswordAsync(userMap, registerDto.Password);
				await _userManager.AddToRoleAsync(userMap, role);
				return true;
			}
		}
		if (role == "Provider")
		{
			var userMap = _mapper.Map<Provider>(registerDto);
			userMap.Email = registerDto.Email;
			userMap.SecurityStamp = Guid.NewGuid().ToString();
			userMap.UserName = registerDto.Email;
			if (_providerRepository.CreateProvider(userMap))
			{
				await _userManager.AddPasswordAsync(userMap, registerDto.Password);
				await _userManager.AddToRoleAsync(userMap, role);
				return true;
			}
		}
		return false;
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
