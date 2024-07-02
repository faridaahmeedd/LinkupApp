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
		string GenerateCode();
		void StoreOtp(string userEmail, string otp);
		Task<bool> VerifyOtp(string userEmail, string otp);
		Task<IdentityResult> CreateAdmin(RegistrationDto registerDto);
		Task<(string Token, DateTime Expiration)> LoginUser(LoginDto loginDto);
		Task<(string Token, DateTime Expiration)> LoginGoogleUser(GoogleJsonWebSignature.Payload payload);
		Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken);
		bool CheckValidPassword(IEnumerable<IdentityError> errors);
		Task<string> ForgetPassword(string mail);
		Task<bool> ResetPassword(string mail, string newPassword);
		Task<bool> DeactivateUser(string userId, string reason);
		bool SendRegistratationMail(string recipientEmail, string otp);
		bool SendWarningEmail(string recipientEmail);
		bool SendDeactivationEmail(string recipientEmail, string reason);
		bool SendResetPasswordEmail(string recipientEmail, string resetCode);
	}
}