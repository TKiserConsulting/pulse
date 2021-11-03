namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using Webfarm.Sdk.Data;

    public abstract class BaseEntity : IIdentified<Guid> 
    {
        [NotMapped]
        public abstract Guid Id { get; set; }
    }
}
