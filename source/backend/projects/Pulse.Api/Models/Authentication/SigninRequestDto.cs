namespace Pulse.Api.Models.Authentication
{
    using System.ComponentModel.DataAnnotations;

    public class SigninRequestDto
    {
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
