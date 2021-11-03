namespace Webfarm.Sdk.Persistence.Migrations.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    public static class AssemblyResourceExtensions
    {
        #region Public Methods and Operators

        [NotNull]
        public static IEnumerable<string> GetStatements([NotNull] this Assembly assembly, [NotNull] string scriptName)
        {
            var sqlText = assembly.GetText(scriptName);
            var result =
                sqlText
                    .Split(
                        new[] { "\r\n/\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.TrimEnd(' ', '/'))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();

            return result;
        }

        [NotNull]
        public static string GetText([NotNull] this Assembly assembly, [NotNull] string scriptName)
        {
            var embeddedResourceName = GetQualifiedResourcePath(scriptName, assembly);

            using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
            {
                Debug.Assert(stream != null, nameof(stream) + " != null");
#pragma warning disable CA1508 // Avoid dead conditional code
                using (var reader = new StreamReader(stream))
#pragma warning restore CA1508 // Avoid dead conditional code
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion

        #region Methods

        [NotNull]
        private static string GetQualifiedResourcePath([NotNull] string name, [NotNull] Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames();

            var parts = name.Split('.').Reverse().ToArray();
            bool IsNameMatch(string x) =>
                x.Split('.').Reverse().Take(parts.Length).SequenceEqual(
                    parts, StringComparer.InvariantCultureIgnoreCase);

            var foundResources = resources.Where(IsNameMatch).ToArray();

            if (foundResources.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Could not find resource named {name} in assembly {assembly.FullName}");
            }

            if (foundResources.Length > 1)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        @"Could not find unique resource named {0} in assembly {1}.\r\nPossible candidates are: \r\n{2}",
                        name,
                        assembly.FullName,
                        string.Join("\r\n\t", foundResources)));
            }

            return foundResources[0];
        }

        #endregion
    }
}
