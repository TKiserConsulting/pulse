namespace Pulse.Api.Models.Students
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Authentication;

    public class JoinSessionResultDto
    {
        [Required]
        public TokenDto TokenInfo { get; set; }

        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public Guid SessionStudentId { get; set; }
    }
}
