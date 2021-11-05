namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SessionEmoticonDetailsDto
    {
        [Required]
        public Guid InstructorEmoticonId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public int TapCount { get; set; }
    }
}
