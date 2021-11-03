namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Session : BaseAuditable
    {
        [NotMapped]
        public override Guid Id
        {
            get => this.SessionId;
            set => this.SessionId = value;
        }

        [Key]
        public Guid SessionId { get; set; }

        public string Code { get; set; }

        public DateTimeOffset? Finished { get; set; }

        public Guid ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public TheClass Class { get; set; }
    }
}
