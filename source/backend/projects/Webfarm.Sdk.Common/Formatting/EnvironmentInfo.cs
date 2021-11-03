namespace Webfarm.Sdk.Common.Formatting
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Common.Extensions;

    public class EnvironmentInfo
    {
        private EnvironmentInfo()
        {
            this.ProcessArchitecture = GetSafeString(GetProcessArchitecture);
            this.OperatingSystemVersion = GetSafeString(GetOsVersion);
            this.OperatingSystemPlatform = GetSafeString(GetOsPlatform);
            this.OperatingSystemArchitecture = GetSafeString(GetOsArchitecture);
            this.CpuCount = GetSafeString(() => Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture));
            this.MachineName = GetSafeString(() => Environment.MachineName);
            this.FrameworkDescription = GetSafeString(GetFrameworkDescription);
            var entryAssembly = Assembly.GetEntryAssembly();
            this.EntryAssemblyName = GetSafeString(() =>
            {
                var assembly = entryAssembly;
                return ((object)assembly != null ? assembly.GetName().Name : (string)null) ?? "unknown";
            });
            this.EntryAssemblyVersion = GetSafeString(() =>
            {
                var assembly = entryAssembly;
                return ((object)assembly != null ? assembly.EffectiveVersion() : (string)null) ?? "unknown";
            });
        }

        public static EnvironmentInfo Instance { get; } = new EnvironmentInfo();

        public string EntryAssemblyName { get; }

        public string EntryAssemblyVersion { get; }

        public string FrameworkDescription { get; }

        public string MachineName { get; }

        public string OperatingSystemArchitecture { get; }

        public string OperatingSystemPlatform { get; }

        public string OperatingSystemVersion { get; }

        public string ProcessArchitecture { get; }

        public string CpuCount { get; }

        private static string GetOsPlatform()
        {
            var platform1 = OSPlatform.Create("Other Platform");
            var platform2 = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OSPlatform.Windows : platform1;
            var platform3 = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSPlatform.OSX : platform2;
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OSPlatform.Linux : platform3).ToString();
        }

        [NotNull]
        private static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString();
        }

        [NotNull]
        private static string GetOsArchitecture()
        {
            return RuntimeInformation.OSArchitecture.ToString();
        }

        private static string GetOsVersion()
        {
            return RuntimeInformation.OSDescription;
        }

        private static string GetFrameworkDescription()
        {
            return RuntimeInformation.FrameworkDescription;
        }

        [NotNull]
        private static string GetSafeString(Func<string> action)
        {
            #pragma warning disable CA1031
            // ReSharper disable CA1031
            try
            {
                return action()?.Trim() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }

            // ReSharper restore CA1031
            #pragma warning restore CA1031
        }
    }
}
