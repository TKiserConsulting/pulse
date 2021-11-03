namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EmoticonTapDto
    {
        [Required]
        public Guid InstructorEmoticonId { get; set; }
    }
}
