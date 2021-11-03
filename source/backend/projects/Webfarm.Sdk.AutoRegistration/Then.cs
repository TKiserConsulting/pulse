namespace Webfarm.Sdk.AutoRegistration
{
    using JetBrains.Annotations;

    #pragma warning disable CA1716 // Identifiers should not match keywords
    public static class Then
    #pragma warning restore CA1716 // Identifiers should not match keywords
    {
        [NotNull]
        public static IFluentRegistration Register()
        {
            return new RegistrationOptions();
        }
    }
}
