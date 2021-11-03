namespace Pulse.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseAuditable : BaseEntity, IAuditable
    {
        protected BaseAuditable()
        {
            this.Created = DateTimeOffset.Now;
        }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        [MaxLength(100)]
        public string ModifiedBy { get; set; }
    }
}
