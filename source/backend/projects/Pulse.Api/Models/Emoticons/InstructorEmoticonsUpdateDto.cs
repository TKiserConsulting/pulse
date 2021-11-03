namespace Pulse.Api.Models.Emoticons
{
    using System.ComponentModel.DataAnnotations;

    public class InstructorEmoticonsUpdateDto
    {
        [Required]
        public InstructorEmoticonListItemDto[] Items { get; set; }
    }
}
