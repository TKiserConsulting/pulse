namespace Pulse.Api.Models.Authentication
{
    using System.ComponentModel.DataAnnotations;

    public class RefreshResultDto
    {
        [Required]
        public TokenDto TokenInfo { get; set; }
    }
}
