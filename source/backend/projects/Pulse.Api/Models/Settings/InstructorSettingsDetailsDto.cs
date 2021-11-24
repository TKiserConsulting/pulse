namespace Pulse.Api.Models.Settings
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class InstructorSettingsDetailsDto
    {
        [Required]
        public Guid InstructorId { get; set; }

        [Required]
        public decimal SessionTimeoutHours { get; set; }

        [Required]
        public decimal EmoticonTapDelaySeconds { get; set; }
    }
}
