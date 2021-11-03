namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SessionCheckin
    {
        [Key]
        public Guid SessionCheckinId { get; set; }

        public CheckinType CheckinType { get; set; }

        public DateTimeOffset Started { get; set; }

        public DateTimeOffset? Finished { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
