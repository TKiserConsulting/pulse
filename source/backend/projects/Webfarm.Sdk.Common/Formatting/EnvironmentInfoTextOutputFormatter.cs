namespace Webfarm.Sdk.Common.Formatting
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public class EnvironmentInfoTextOutputFormatter
    {
        private const int Padding = 25;
        private const char Separator = '=';

        private readonly string applicationName;

        public EnvironmentInfoTextOutputFormatter(string applicationName = null)
        {
            this.applicationName = applicationName;
        }

        public async Task WriteAsync(
            [NotNull] Stream output,
            CancellationToken cancellationToken = default)
        {
            var environmentInfo = EnvironmentInfo.Instance;
            #pragma warning disable CA2000
            await using var sw = new StreamWriter(output, Encoding.UTF8, -1, true);
            #pragma warning restore CA2000
            await this.Write(
                sw,
                "Application Name",
                this.applicationName ?? environmentInfo.EntryAssemblyName,
                cancellationToken);
            await this.Write(sw, "Application Version", environmentInfo.EntryAssemblyVersion, cancellationToken);
            await this.Write(sw, "Assembly Name", environmentInfo.EntryAssemblyName, cancellationToken);
            await this.Write(sw, "Framework Description", environmentInfo.FrameworkDescription, cancellationToken);
            await this.Write(sw, "Local Time", DateTime.Now.ToString("O", CultureInfo.InvariantCulture), cancellationToken);
            await this.Write(sw, "Machine Name", environmentInfo.MachineName, cancellationToken);
            await this.Write(sw, "OS Architecture", environmentInfo.OperatingSystemArchitecture, cancellationToken);
            await this.Write(sw, "OS Platform", environmentInfo.OperatingSystemPlatform, cancellationToken);
            await this.Write(sw, "OS Version", environmentInfo.OperatingSystemVersion, cancellationToken);
            await this.Write(sw, "Process Architecture", environmentInfo.ProcessArchitecture, cancellationToken);
            await this.Write(sw, "Cpu Count", environmentInfo.CpuCount, cancellationToken);
        }

        private async Task Write([NotNull] TextWriter writer, [NotNull] string name, string value, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();
            this.PaddedFormat(sb, name, value);
            sb.Append('\n');
            await writer.WriteAsync(sb, cancellationToken);
        }

        private void PaddedFormat([NotNull] StringBuilder sb, [NotNull] string label, string value)
        {
            if (label.Length + 2 + /*Separator.Length*/ 1 < Padding)
            {
                sb.Append(new string(' ', Padding - label.Length - 1 - /*Separator.Length*/ 1));
            }

            sb.Append(label);
            sb.Append(' ');
            sb.Append(Separator);
            sb.Append(' ');
            sb.Append(value);
        }
    }
}
