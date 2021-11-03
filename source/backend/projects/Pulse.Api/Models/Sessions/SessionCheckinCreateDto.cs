namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public class SessionCheckinCreateDto
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public CheckinType CheckinType { get; set; }
    }
}
