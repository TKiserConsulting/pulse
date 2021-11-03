namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SessionCreateDto
    {
        [Required]
        public Guid ClassId { get; set; }
    }
}
