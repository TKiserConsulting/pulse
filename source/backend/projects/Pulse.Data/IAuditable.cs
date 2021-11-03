namespace Pulse.Data
{
    using System;

    public interface IAuditable
    {
        DateTimeOffset Created { get; set; }

        DateTimeOffset? Modified { get; set; }

        string CreatedBy { get; set; }

        string ModifiedBy { get; set; }
    }
}
