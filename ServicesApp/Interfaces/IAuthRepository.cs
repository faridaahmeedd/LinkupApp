using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using ServicesApp.Dto.Authentication;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IAuthRepository
	{
		Task<AppUser?> CheckUser(string email);
		Task<AppUser?> CheckUserById(string id);
		Task<AppUser?> CheckAdmin(string email);
		Task<bool> CheckRole(string role);
		Task<IdentityResult> CreateUser(RegistrationDto registerDto, string role);
		Task<IdentityResult> CreateAdmin(RegistrationDto registerDto);
		Task<(string Token, DateTime Expiration)> LoginUser(LoginDto loginDto);
		Task<(string Token, DateTime Expiration)> LoginGoogleUser(GoogleJsonWebSignature.Payload payload);
		Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken);
		bool CheckValidPassword(IEnumerable<IdentityError> errors);
		Task<string> ForgetPassword(string mail);
		string GenerateRandomCode(int length);
		Task<bool> ResetPassword(string mail, string newPassword);
		Task<bool> DeactivateUser(string userId);
		bool SendMail(string recipientEmail, string subject, string filename);
		bool SendResetPasswordEmail(string recipientEmail, string resetCode);
		bool SendRegistrtationMail(string recipientEmail);
	}
}