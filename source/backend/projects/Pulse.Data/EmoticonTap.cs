namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmoticonTap
    {
        [Key]
        public Guid EmoticonTapId { get; set; }

        public Guid SessionStudentId { get; set; }

        [ForeignKey(nameof(SessionStudentId))]
        public SessionStudent SessionStudent { get; set; }

        public Guid InstructorEmoticonId { get; set; }

        [ForeignKey(nameof(InstructorEmoticonId))]
        public InstructorEmoticon Emoticon { get; set; }

        public long TimeTicks { get; set; }
    }
}
