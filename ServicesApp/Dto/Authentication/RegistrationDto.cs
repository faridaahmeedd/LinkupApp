using System.ComponentModel.DataAnnotations;

namespace ServicesApp.Dto.Authentication
{
    public class RegistrationDto
    {
		[EmailAddress]
		public required string Email { get; set; }
        public required string Password { get; set; }

        [Compare("Password")]
        public required string ConfirmPassword { get; set; }
    }
}
