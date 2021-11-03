namespace Pulse.Api.Models.Emoticons
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class InstructorEmoticonListItemDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public int SortIndex { get; set; }
    }
}
