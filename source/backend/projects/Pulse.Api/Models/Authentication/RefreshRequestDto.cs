namespace Pulse.Api.Models.Authentication
{
    using System.ComponentModel.DataAnnotations;

    public class RefreshRequestDto
    {
        [Required]
        public TokenDto TokenInfo { get; set; }
    }
}
