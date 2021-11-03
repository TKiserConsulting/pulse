namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SessionCheckinFinishDto
    {
        [Required]
        public Guid SessionCheckinId { get; set; }
    }
}
