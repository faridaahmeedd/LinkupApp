using System.ComponentModel.DataAnnotations;

namespace ServicesApp.Dto.Authentication
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
