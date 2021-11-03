namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public class SessionCheckinDetailsDto
    {
        [Required]
        public Guid SessionCheckinId { get; set; }

        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public CheckinType CheckinType { get; set; }

        public SessionQuestionDetailsDto[] Questions { get; set; }
    }
}
