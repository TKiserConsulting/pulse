namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SessionQuestionCreateDto
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
