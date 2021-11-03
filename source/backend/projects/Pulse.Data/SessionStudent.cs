namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SessionStudent : BaseAuditable
    {
        [NotMapped]
        public override Guid Id
        {
            get => this.SessionStudentId;
            set => this.SessionStudentId = value;
        }

        [Key]
        public Guid SessionStudentId { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
