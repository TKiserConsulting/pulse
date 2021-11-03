namespace Pulse.Api.Models.Users
{
    using System.ComponentModel.DataAnnotations;
    using Authentication;

    public class RegisterResultDto
    {
        [Required]
        public UserDetailsDto User { get; set; }

        [Required]
        public TokenDto TokenInfo { get; set; }
    }
}
