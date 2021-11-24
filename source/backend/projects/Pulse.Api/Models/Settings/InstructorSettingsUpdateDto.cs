namespace Pulse.Api.Models.Settings
{
    using System.ComponentModel.DataAnnotations;

    public class InstructorSettingsUpdateDto
    {
        [Required]
        public decimal SessionTimeoutHours { get; set; }

        [Required]
        public decimal EmoticonTapDelaySeconds { get; set; }
    }
}
