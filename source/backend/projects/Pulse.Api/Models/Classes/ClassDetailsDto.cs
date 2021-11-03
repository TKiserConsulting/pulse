namespace Pulse.Api.Models.Classes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Webfarm.Sdk.Data;

    public class ClassDetailsDto : IIdentified<Guid>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Guid InstructorId { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset Created { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }
    }
}
