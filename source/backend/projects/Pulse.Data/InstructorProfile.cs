namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Auth;

    public class InstructorProfile : BaseAuditable
    {
        [NotMapped]
        public override Guid Id
        {
            get => this.InstructorId;
            set => this.InstructorId = value;
        }

        [Key]
        public Guid InstructorId { get; set; }

        [MaxLength(100)]
        public string School { get; set; }

        [MaxLength(100)]
        public string Subject { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        public byte[] Image { get; set; }

        public byte[] SmallImage { get; set; }

        [ForeignKey(nameof(InstructorId))]
        public ApplicationUser Instructor { get; set; }
    }
}
