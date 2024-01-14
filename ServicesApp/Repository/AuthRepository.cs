﻿using AutoMapper;
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
using System.IO;


public class AuthRepository
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
		Console.WriteLine(userMap);
        if (role == "Customer")
		{
			 userMap = _mapper.Map<Customer>(registerDto);
        }
		else if (role == "Provider")
		{
			 userMap = _mapper.Map<Provider>(registerDto);
			
		}
		else if (role == "Admin")
		{
			 userMap = _mapper.Map<Admin>(registerDto);
			
		}
		
        userMap.Email = registerDto.Email;
        userMap.SecurityStamp = Guid.NewGuid().ToString();
        userMap.UserName = new MailAddress(registerDto.Email).User;
		var result = await _userManager.CreateAsync(userMap, registerDto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(userMap, role);
            string senderEmail = "linkupp2024@gmail.com";
            string senderPassword = "mbyo noyk dfbb fhlr";
            string recipientEmail = userMap.Email;
            MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail)
            {
                 Subject = "Welcome to Linkup Service Hub",
             
                 Body = File.ReadAllText("Mails/RegistrationMail.html"),
                 IsBodyHtml = true,
            };

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

             smtpClient.Send(mailMessage);
            }
        return result;
	}


	public async Task<(string Token, DateTime Expiration , string Roles)> LoginUser(LoginDto loginDto)
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
            Console.WriteLine(roles );

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
			return (token, expiration , roles.FirstOrDefault() );
		}
		return (null, DateTime.MinValue , null);
	}


    public async  Task<string> ForgetPassword( string mail )
	{
        string senderEmail = "linkupp2024@gmail.com";
        string senderPassword = "mbyo noyk dfbb fhlr"; 
        string recipientEmail = mail;

        var user = await _userManager.FindByEmailAsync(mail);
		if (user == null){
			
			return string.Empty;
		}

        string resetCode = GenerateRandomCode(); 

        // Save the confirmation code in your database or a secure storage
        // user.ConfirmationCode = confirmationCode; 
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

        if (user == null)
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
