namespace Pulse.Api.Models.Sessions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Webfarm.Sdk.Data;

    public class SessionListItemDto : IIdentified<Guid>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Finished { get; set; }
    }
}
