namespace Pulse.Api.Models.Students
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Emoticons;
    using Sessions;

    public class StudentSessionDetailsDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public InstructorEmoticonListItemDto[] Emoticons { get; set; }

        public SessionCheckinDetailsDto ActiveCheckin { get; set; }

        public SessionQuestionDetailsDto[] Questions { get; set; }
    }
}
