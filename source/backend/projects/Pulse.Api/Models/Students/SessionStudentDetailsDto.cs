namespace Pulse.Api.Models.Students
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Webfarm.Sdk.Data;

    public class SessionStudentDetailsDto : IIdentified<Guid>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid SessionId { get; set; }
    }
}
