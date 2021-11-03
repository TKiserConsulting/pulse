namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Auth;

    public class InstructorEmoticon : BaseAuditable
    {
        [NotMapped]
        public override Guid Id
        {
            get => this.InstructorEmoticonId;
            set => this.InstructorEmoticonId = value;
        }

        [Key]
        public Guid InstructorEmoticonId { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(10)]
        public string Color { get; set; }

        public int SortIndex { get; set; }

        public Guid InstructorId { get; set; }

        [ForeignKey(nameof(InstructorId))]
        public ApplicationUser Instructor { get; set; }
    }
}
