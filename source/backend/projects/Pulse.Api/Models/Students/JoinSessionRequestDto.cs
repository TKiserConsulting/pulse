namespace Pulse.Api.Models.Students
{
    using System.ComponentModel.DataAnnotations;

    public class JoinSessionRequestDto
    {
        [Required]
        [MaxLength(10)]
        public string SessionCode { get; set; }
    }
}
