namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Auth;

    public class TheClass : BaseAuditable
    {
        [NotMapped]
        public override Guid Id
        {
            get => this.ClassId;
            set => this.ClassId = value;
        }

        [Key]
        public Guid ClassId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public Guid InstructorId { get; set; }

        [ForeignKey(nameof(InstructorId))]
        public ApplicationUser Instructor { get; set; }
    }
}
