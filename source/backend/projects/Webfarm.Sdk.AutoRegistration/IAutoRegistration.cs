// ReSharper disable UnusedMember.Global
namespace Webfarm.Sdk.AutoRegistration
{
    using System;
    using System.Reflection;

    public interface IAutoRegistration
    {
        IAutoRegistration IncludeAssembly(Assembly assembly);

        IAutoRegistration ExcludeAssemblies(Predicate<Assembly> filter);

        IAutoRegistration Include(
            Predicate<Type> typeFilter,
            IRegistrationOptions registrationOptions);

        IAutoRegistration Include(
            Predicate<Type> typeFilter,
            Action<Type> registrator);

        IAutoRegistration Exclude(Predicate<Type> filter);

        void ApplyAutoRegistration();
    }
}
