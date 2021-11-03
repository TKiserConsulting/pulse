namespace Pulse.Api.Models.Authentication
{
    using System.ComponentModel.DataAnnotations;
    using Users;

    public class SigninResultDto
    {
        [Required]
        public TokenDto TokenInfo { get; set; }

        [Required]
        public UserDetailsDto User { get; set; }
    }
}
