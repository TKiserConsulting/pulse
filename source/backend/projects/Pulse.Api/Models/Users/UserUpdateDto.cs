namespace Pulse.Api.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public class UserUpdateDto
    {
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }
    }
}
