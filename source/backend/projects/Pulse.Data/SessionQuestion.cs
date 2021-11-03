namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SessionQuestion
    {
        [Key]
        public Guid SessionQuestionId { get; set; }

        public Guid SessionCheckinId { get; set; }

        [ForeignKey(nameof(SessionCheckinId))]
        public SessionCheckin SessionCheckin { get; set; }

        public Guid SessionStudentId { get; set; }

        [ForeignKey(nameof(SessionStudentId))]
        public SessionStudent SessionStudent { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Dismissed { get; set; }

        [MaxLength(500)]
        public string Text { get; set; }
    }
}
