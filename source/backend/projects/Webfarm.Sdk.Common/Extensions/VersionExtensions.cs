namespace Webfarm.Sdk.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    public static class VersionExtensions
    {
        private const string DefaultVersionNumber = "1.0.0.0";

        [NotNull]
        public static string EffectiveVersion([NotNull] this Type type)
        {
            return EffectiveVersion(type.Assembly);
        }

        [NotNull]
        public static string EffectiveVersion([NotNull] this Assembly assembly)
        {
            return GetVersionOptions(assembly).First();
        }

        [ItemNotNull]
        private static IEnumerable<string> GetVersionOptions([NotNull] Assembly assembly)
        {
            var version = assembly
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .Select(a => a.InformationalVersion).FirstOrDefault();
            if (!string.IsNullOrEmpty(version))
            {
                yield return version;
            }

            version = assembly
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                .OfType<AssemblyFileVersionAttribute>()
                .Select(a => a.Version).FirstOrDefault();
            if (!string.IsNullOrEmpty(version))
            {
                yield return version;
            }

            version = assembly
                .GetName()
                .Version?.ToString();
            if (!string.IsNullOrEmpty(version))
            {
                yield return version;
            }

            yield return DefaultVersionNumber;
        }
    }
}
