namespace Pulse.Api.Models.Emoticons
{
    using System.ComponentModel.DataAnnotations;

    public class InstructorEmoticonUpdateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Color { get; set; }
    }
}
