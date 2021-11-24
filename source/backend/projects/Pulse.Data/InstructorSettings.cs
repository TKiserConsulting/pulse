namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Auth;

    public class InstructorSettings : BaseAuditable
    {
        public const decimal DefaultSessionTimeoutHours = 2;

        public const decimal DefaultEmoticonTapDelaySeconds = 4;

        [NotMapped]
        public override Guid Id
        {
            get => this.InstructorId;
            set => this.InstructorId = value;
        }

        [Key]
        public Guid InstructorId { get; set; }

        [ForeignKey(nameof(InstructorId))]
        public ApplicationUser Instructor { get; set; }

        public decimal SessionTimeoutHours { get; set; }

        public decimal EmoticonTapDelaySeconds { get; set; }
    }
}
