namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SessionQuestionDetailsDto
    {
        [Required]
        public Guid SessionQuestionId { get; set; }

        [Required]
        public Guid SessionCheckinId { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Dismissed { get; set; }
    }
}
