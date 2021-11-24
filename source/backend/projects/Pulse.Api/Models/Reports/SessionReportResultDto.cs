namespace Pulse.Api.Models.Reports
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Classes;
    using Emoticons;
    using Sessions;

    public class SessionReportResultDto
    {
        [Required]
        public ClassListItemDto Class { get; set; }

        [Required]
        public SessionListItemDto Session { get; set; }

        [Required]
        public EmoticonData[] Emoticons { get; set; }

        [Required]
        public CheckinData[] Checkins { get; set; }

        [Required]
        public int IntervalMinutes { get; set; }
    }

    public class EmoticonData
    {
        [Required]
        public InstructorEmoticonListItemDto Emoticon { get; set; }

        [Required]
        public EmoticonTapData[] Taps { get; set; }
    }

    public class EmoticonTapData
    {
        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        public int TapCount { get; set; }
    }

    public class CheckinData
    {
        [Required]
        public SessionCheckinDetailsDto Checkin { get; set; }

        [Required]
        public SessionQuestionDetailsDto[] Questions { get; set; }
    }
}
