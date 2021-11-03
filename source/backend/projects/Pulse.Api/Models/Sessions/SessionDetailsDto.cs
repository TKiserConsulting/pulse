namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Webfarm.Sdk.Data;

    public class SessionDetailsDto : IIdentified<Guid>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public string Code { get; set; }
    
        public DateTimeOffset? Finished { get; set; }

        public string CreatedBy { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }
    }
}
