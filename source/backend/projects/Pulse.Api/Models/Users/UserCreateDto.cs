namespace Pulse.Api.Models.Users
{
    using System.ComponentModel.DataAnnotations;
    using Data;
    using Data.Auth;

    public class UserCreateDto
    {
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
