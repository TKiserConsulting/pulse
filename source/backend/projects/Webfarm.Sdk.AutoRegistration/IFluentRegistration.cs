// ReSharper disable UnusedMember.Global
namespace Webfarm.Sdk.AutoRegistration
{
    using System;
    using DryIoc;

    public interface IFluentRegistration : IRegistrationOptions
    {
#pragma warning disable CA1716 // Identifiers should not match keywords
        IFluentRegistration WithName(string name);

        IFluentRegistration WithName(Func<Type, string> nameResolver);

        IFluentRegistration WithTypeName();

        IFluentRegistration WithPartName(string name);

        IFluentRegistration WithReuse(IReuse reuse);

        IFluentRegistration As<TContact>()
            where TContact : class;

        IFluentRegistration As(Func<Type, Type> typeResolver);

        IFluentRegistration As(Func<Type, Type[]> typesResolver);

        IFluentRegistration AsFirstInterfaceOfType();

        IFluentRegistration AsSingleInterfaceOfType();

        IFluentRegistration AsAllInterfacesOfType();

        IFluentRegistration For(Func<Type, Type> typeResolver);
#pragma warning restore CA1716 // Identifiers should not match keywords
    }
}
