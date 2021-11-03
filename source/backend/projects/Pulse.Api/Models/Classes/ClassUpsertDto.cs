namespace Pulse.Api.Models.Classes
{
    using System.ComponentModel.DataAnnotations;

    public class ClassUpsertDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
