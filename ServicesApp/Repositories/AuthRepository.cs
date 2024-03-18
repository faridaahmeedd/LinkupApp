using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.Core.Models;
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

public class AuthRepository : IAuthRepository
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IConfiguration _config;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IMapper _mapper;

	public AuthRepository(
		UserManager<AppUser> userManager,
		IConfiguration config,
		RoleManager<IdentityRole> roleManager,
		IMapper mapper)
	{
		_userManager = userManager;
		_config = config;
		_roleManager = roleManager;
		_mapper = mapper;
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
			if(role.Contains("Admin"))
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
            SendRegistrtationMail(userMap.Email);
        }
        return result;
    }

	public async Task<IdentityResult> CreateAdmin(RegistrationDto registerDto)
	{
		var userMap = _mapper.Map<Admin>(registerDto);
		userMap.Email = registerDto.Email;
		userMap.SecurityStamp = Guid.NewGuid().ToString();
		userMap.UserName = registerDto.Email;
		var result = await _userManager.CreateAsync(userMap, registerDto.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(userMap, "Admin");
		}
		return result;
	}

	public void SendRegistrtationMail(string recipientEmail)
	{
        string senderEmail = "linkupp2024@gmail.com";
        string senderPassword = "mbyo noyk dfbb fhlr";
        LinkedResource LinkedImage = new LinkedResource(@"wwwroot\images\Logo.png");
        LinkedImage.ContentId = "Logo";
        LinkedImage.ContentType = new ContentType(MediaTypeNames.Image.Png);
        string htmlContent = File.ReadAllText("Mails/RegistrationMail.html");
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
        htmlContent, null, "text/html");
        htmlView.LinkedResources.Add(LinkedImage);

        MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
        {
            Subject = "Welcome to Linkup",
            IsBodyHtml = true,
        };
        mailMessage.AlternateViews.Add(htmlView);

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };
		try
		{
			smtpClient.Send(mailMessage);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
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
		if (user == null){
			return string.Empty;
		}

		var resetCode = GenerateRandomCode();

		string senderEmail = "linkupp2024@gmail.com";
		string senderPassword = "mbyo noyk dfbb fhlr";
		string recipientEmail = mail;
		LinkedResource LinkedImage = new LinkedResource(@"wwwroot\images\Logo.png");
		LinkedImage.ContentId = "Logo";
		LinkedImage.ContentType = new ContentType(MediaTypeNames.Image.Png);
		string htmlContent = File.ReadAllText("Mails/ResetPassMail.html");
		htmlContent = htmlContent.Replace("{ResetCodePlaceholder}", resetCode);
		AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
	    htmlContent, null, "text/html");
		htmlView.LinkedResources.Add(LinkedImage);

		MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
		{
			Subject = "Linkup Reset Password",
			IsBodyHtml = true, 
        };
		mailMessage.AlternateViews.Add(htmlView);
		SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };
        try
        {
            smtpClient.Send(mailMessage);
			smtpClient.Dispose();
            return resetCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return string.Empty;
	}

    public string GenerateRandomCode(int length = 6)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
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

	public async Task<bool> DeactivateUser(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user != null)
		{
			var role = await _userManager.GetRolesAsync(user);
			if (! role.Contains("MainAdmin"))
			{
				user.Active = false;
				var result = await _userManager.UpdateAsync(user);

				if (result.Succeeded)
				{
					SendMail(user.Email , "Linkup Deactivation", "InactiveMail");
                    return true;
				}
			}
		}
		
		return false;
	}
    public void SendMail(string recipientEmail , string subject , string filename)
	{
        string senderEmail = "linkupp2024@gmail.com";
        string senderPassword = "mbyo noyk dfbb fhlr";
     
        LinkedResource LinkedImage = new LinkedResource(@"wwwroot\images\Logo.png");
        LinkedImage.ContentId = "Logo";
        LinkedImage.ContentType = new ContentType(MediaTypeNames.Image.Png);
        string htmlContent = File.ReadAllText($"Mails/{filename}.html");
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
        htmlContent, null, "text/html");
        htmlView.LinkedResources.Add(LinkedImage);

        MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
        {
            Subject = subject,
            IsBodyHtml = true,
        };
        mailMessage.AlternateViews.Add(htmlView);
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };
        try
        {
            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }

}