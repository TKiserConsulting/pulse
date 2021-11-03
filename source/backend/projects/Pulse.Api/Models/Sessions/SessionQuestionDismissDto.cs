namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SessionQuestionDismissDto
    {
        [Required]
        public Guid SessionQuestionId { get; set; }
    }
}
