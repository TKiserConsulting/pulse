namespace Pulse.Api.Models.Profiles
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class InstructorProfileDetailsDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string School { get; set; }

        public string Subject { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}
