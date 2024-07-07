using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.Dto.Authentication;
using ServicesApp.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using ServicesApp.Interfaces;
using System.Net.Mime;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.EmailSetting;
using ServicesApp.Setting;

public class AuthRepository : IAuthRepository
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IConfiguration _config;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IMapper _mapper;
	private readonly IProviderRepository _providerRepository;
	private readonly IMemoryCache _cache;
	private readonly EmailSetting _emailSetting;
    private readonly GoogleSetting _googleSetting ;


    public AuthRepository(
		UserManager<AppUser> userManager,
		IConfiguration config,
		RoleManager<IdentityRole> roleManager,
		IMapper mapper,
		IProviderRepository providerRepository,
		IMemoryCache cache,
		IOptions<EmailSetting> emailSetting , IOptions<GoogleSetting> googleSetting)
	{
		_userManager = userManager;
		_config = config;
		_roleManager = roleManager;
		_mapper = mapper;
		_providerRepository = providerRepository;
		_cache = cache;
		_emailSetting = emailSetting.Value;
		_googleSetting = googleSetting.Value;
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

	public async Task<AppUser?> CheckUserById(string id)
	{
		var appUser = await _userManager.FindByIdAsync(id);
		if (appUser != null)
		{
			return appUser;
		}
		return null;
	}

	public async Task<AppUser?> CheckAdmin(string email)
	{
		var appUser = await _userManager.FindByEmailAsync(email);
		if (appUser != null)
		{
			var role = await _userManager.GetRolesAsync(appUser);
			if (role.Contains("Admin"))
			{
				return appUser;
			}
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

	public async Task<IdentityResult> CreateUser(RegistrationDto registerDto, string role)
	{
		var userMap = _mapper.Map<AppUser>(registerDto);
		if (role == "Customer")
		{
			userMap = _mapper.Map<Customer>(registerDto);
		}
		else if (role == "Provider")
		{
			userMap = _mapper.Map<Provider>(registerDto);
		}
		else
		{
			return IdentityResult.Failed();
		}
		userMap.Email = registerDto.Email;
		userMap.SecurityStamp = Guid.NewGuid().ToString();
		userMap.UserName = registerDto.Email;
		var result = await _userManager.CreateAsync(userMap, registerDto.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(userMap, role);

			string otp = GenerateCode();
			StoreOtp(userMap.Email, otp);

			if (SendRegistratationMail(userMap.Email, otp))
			{
				return result;
			}
		}
		return result;
	}

	public string GenerateCode()
	{
		Random random = new Random();
		return random.Next(100000, 999999).ToString();  // Generate a 6-digit OTP
	}

	public void StoreOtp(string userEmail, string otp)
	{
		_cache.Set(userEmail, otp, TimeSpan.FromMinutes(10)); // Store OTP in cache for 10 minutes
	}

	public async Task<bool> VerifyOtp(string userEmail, string otp)
	{
		if (_cache.TryGetValue(userEmail, out string storedOtp))
		{
			if(storedOtp == otp)
			{
				var user = await CheckUser(userEmail);
				if(user != null)
				{
					user.EmailConfirmed = true;
					await _userManager.UpdateAsync(user);
					return true;
				}
			}
		}
		return false;
	}

	public async Task<IdentityResult> CreateAdmin(RegistrationDto registerDto)
	{
		var userMap = _mapper.Map<Admin>(registerDto);
		userMap.Email = registerDto.Email;
		userMap.EmailConfirmed = true;
		userMap.SecurityStamp = Guid.NewGuid().ToString();
		userMap.UserName = registerDto.Email;
		var result = await _userManager.CreateAsync(userMap, registerDto.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(userMap, "Admin");
		}
		return result;
	}

	public bool CheckValidPassword(IEnumerable<IdentityError> errors)
	{
		foreach (var error in errors)
		{
			if (error.Code.StartsWith("Password"))
			{
				return false;
			}
		}
		return true;
	}

	public async Task<(string Token, DateTime Expiration)> LoginUser(LoginDto loginDto)
	{
		var appUser = await _userManager.FindByEmailAsync(loginDto.Email);
		if (appUser != null && appUser.Active && await _userManager.CheckPasswordAsync(appUser, loginDto.Password))
		{
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, appUser.Email),
				new Claim(ClaimTypes.NameIdentifier, appUser.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
			var roles = await _userManager.GetRolesAsync(appUser);
			if(!_providerRepository.CheckApprovedProvider(appUser.Id))
			{
				return (null, DateTime.MinValue);
			}
			authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var expiration = DateTime.Now.AddMonths(5);

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

	public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
	{
		try
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				Audience = new List<string> { _googleSetting.ClientId }
			};
			var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
			return payload;
		}
		catch (InvalidJwtException ex)
		{
			Console.WriteLine("Invalid JWT exception: " + ex.Message);
			return null;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Exception occurred during token validation: " + ex.Message);
			return null;
		}
	}

	public async Task<(string Token, DateTime Expiration)> LoginGoogleUser(GoogleJsonWebSignature.Payload payload)
	{
		var appUser = await _userManager.FindByEmailAsync(payload.Email);
		if (appUser != null && appUser.Active)
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
			var expiration = DateTime.Now.AddMonths(5);

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


	public async Task<string> ForgetPassword(string mail)
	{
		var user = await _userManager.FindByEmailAsync(mail);
		if (user == null)
		{
			return string.Empty;
		}

		var resetCode = GenerateCode();
		if (SendResetPasswordEmail(mail, resetCode))
		{
			return resetCode;
		}
		return resetCode;
	}

	public async Task<bool> ResetPassword(string mail, string newPassword)
	{
		var user = await _userManager.FindByEmailAsync(mail);
		if (user == null)
		{
			return false;
		}
		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
		var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
		if (result.Succeeded)
		{
			await _userManager.UpdateAsync(user);
			return true;
		}
		return false;
	}

	public async Task<bool> DeactivateUser(string userId, string reason)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user != null)
		{
			var role = await _userManager.GetRolesAsync(user);
			if (!role.Contains("SuperAdmin"))
			{
				user.Active = false;
				var result = await _userManager.UpdateAsync(user);

				if (result.Succeeded)
				{
					if (SendDeactivationEmail(user.Email, reason))
					{
						return true;
					}
					return true;
				}
			}
		}
		return false;
	}

	private async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlContent)
	{
		string senderEmail = _emailSetting.from;
		string senderPassword = _emailSetting.Password;

		try
		{
			LinkedResource linkedImage = new LinkedResource(@"wwwroot\images\Logo.png")
			{
				ContentId = "Logo",
				ContentType = new ContentType(MediaTypeNames.Image.Png)
			};

			AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContent, null, "text/html");
			htmlView.LinkedResources.Add(linkedImage);

			MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
			{
				Subject = subject,
				IsBodyHtml = true,
			};
			mailMessage.AlternateViews.Add(htmlView);

			using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(senderEmail, senderPassword),
				EnableSsl = true
			})
			{
				await smtpClient.SendMailAsync(mailMessage);
			}

			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			return false;
		}
	}


	public bool SendRegistratationMail(string recipientEmail, string otp)
	{
		try
		{
			string htmlContent = File.ReadAllText(@"wwwroot\Mails\RegistrationMail.html");
			htmlContent = htmlContent.Replace("{OtpPlaceholder}", otp);

			return SendEmailAsync(recipientEmail, "Welcome to Linkup", htmlContent).Result;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			return false;
		}
	}

	public bool SendResetPasswordEmail(string recipientEmail, string resetCode)
	{
		try
		{
			string htmlContent = File.ReadAllText(@"wwwroot\Mails\ResetPassMail.html");
			htmlContent = htmlContent.Replace("{ResetCodePlaceholder}", resetCode);

			return SendEmailAsync(recipientEmail, "Linkup Reset Password", htmlContent).Result;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			return false;
		}
	}

	public bool SendDeactivationEmail(string recipientEmail, string reason)
	{
		try
		{
			string htmlContent = File.ReadAllText(@"wwwroot\Mails\DeactivationMail.html");
			htmlContent = htmlContent.Replace("{ReasonPlaceholder}", reason);

			return SendEmailAsync(recipientEmail, "Linkup Deactivation", htmlContent).Result;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			return false;
		}
	}

	public bool SendWarningEmail(string recipientEmail)
	{
		try
		{
			string htmlContent = File.ReadAllText(@"wwwroot\Mails\WarningMail.html");

			return SendEmailAsync(recipientEmail, "Linkup Warning", htmlContent).Result;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			return false;
		}
	}
}