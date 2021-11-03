namespace Webfarm.Sdk.AutoRegistration.Extensions
{
    using System;
    using DryIoc;
    using JetBrains.Annotations;

    public static class RegistrationExtensions
    {
        [NotNull]
        public static IAutoRegistration ConfigureAutoRegistration([NotNull] this IRegistrator registrator)
        {
            if (registrator == null)
            {
                throw new ArgumentNullException(nameof(registrator));
            }

            return new AutoRegistrationBuilder(registrator);
        }
    }
}
