namespace Pulse.Api.Models.Classes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Webfarm.Sdk.Data;

    public class ClassListItemDto : IIdentified<Guid>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
