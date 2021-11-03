namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Emoticon : BaseAuditable
    {
        [NotMapped]
        public override Guid Id
        {
            get => this.EmoticonId;
            set => this.EmoticonId = value;
        }

        [Key]
        public Guid EmoticonId { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(10)]
        public string Color { get; set; }

        public int SortIndex { get; set; }
    }
}
