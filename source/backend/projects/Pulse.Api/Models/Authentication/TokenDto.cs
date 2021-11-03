namespace Pulse.Api.Models.Authentication
{
    using System.ComponentModel.DataAnnotations;

    public class TokenDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
