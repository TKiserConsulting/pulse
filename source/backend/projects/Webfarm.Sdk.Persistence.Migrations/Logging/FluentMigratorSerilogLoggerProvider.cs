namespace Infoplus.Askod.Sdk.Daemon.Migrations.Logging
{
    using FluentMigrator.Runner;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public sealed class FluentMigratorSerilogLoggerProvider : ILoggerProvider
    {
        private readonly FluentMigratorLoggerOptions options;

        public FluentMigratorSerilogLoggerProvider([NotNull] IOptions<FluentMigratorLoggerOptions> options)
        {
            this.options = options.Value;
        }

        [NotNull]
        public ILogger CreateLogger(string categoryName)
        {
            return new FluentMigratorSerilogLogger(this.options);
        }

        public void Dispose()
        {
            // Nothing to do here...
        }
    }
}
