using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.Core.Models;
using ServicesApp.Dto.Authentication;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System;
public class AuthRepository
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IConfiguration _config;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IMapper _mapper;
	private readonly ICustomerRepository _customerRepository;
	private readonly IProviderRepository _providerRepository;
	private readonly IAdminRepository _adminRepository;

	public AuthRepository(
		UserManager<AppUser> userManager,
		IConfiguration config,
		RoleManager<IdentityRole> roleManager,
		IMapper mapper,
		ICustomerRepository customerRepository,
		IProviderRepository providerRepository,
		IAdminRepository adminRepository)
	{
		_userManager = userManager;
		_config = config;
		_roleManager = roleManager;
		_mapper = mapper;
		_customerRepository = customerRepository;
		_providerRepository = providerRepository;
		_adminRepository = adminRepository;
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
		if (role == "Admin")
		{
			var userMap = _mapper.Map<Admin>(registerDto);
			userMap.Email = registerDto.Email;
			userMap.SecurityStamp = Guid.NewGuid().ToString();
			userMap.UserName = registerDto.Email;
			if (_adminRepository.CreateAdmin(userMap))
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




  
    public async  Task<string> ForgetPassword( string mail )
	{
        string senderEmail = "shrookayman617@gmail.com";
        string senderPassword = "duzi ugle sqrh wtgx"; 
        string recipientEmail = mail;

        var user = await _userManager.FindByEmailAsync(mail);
		if (user == null){
			
			return string.Empty;
		}

        string resetCode = GenerateRandomCode(); 

        // Save the confirmation code in your database or a secure storage
        //user.ConfirmationCode = confirmationCode; 
       // await _userManager.UpdateAsync(user);


        MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
        {
            Subject = "Linkup Reset Password Email",
            Body = $"Reset Code : {resetCode}",
            IsBodyHtml = true , 
        }; 

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };

        try
        {
            smtpClient.Send(mailMessage);
            Console.WriteLine("Email sent successfully!");
            return resetCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return string.Empty;
	}

    private string GenerateRandomCode(int length = 6)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    public async Task<bool> ResetPassword(string mail, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(mail);

        if (user == null )
        {
            return false; 
        }

        // Reset the user's password
       // var jjj = await _userManager.ChangePasswordAsync()
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (result.Succeeded)
        {
           
            await _userManager.UpdateAsync(user);
            return true; 
        }
		return false;
    }



}
