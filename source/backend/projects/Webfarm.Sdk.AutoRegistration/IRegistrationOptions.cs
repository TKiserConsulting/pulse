namespace Webfarm.Sdk.AutoRegistration
{
    using System;
    using System.Collections.Generic;
    using DryIoc;

    public interface IRegistrationOptions
    {
        Type Type { get; set; }

        string Name { get; set; }

        IReuse Reuse { get; set; }

        IEnumerable<Type> Interfaces { get; set; }

        Type Implementation { get; }
    }
}
