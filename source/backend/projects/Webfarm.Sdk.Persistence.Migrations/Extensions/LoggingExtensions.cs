namespace Infoplus.Askod.Sdk.Daemon.Migrations.Extensions
{
    using Infoplus.Askod.Sdk.Daemon.Migrations.Logging;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class LoggingExtensions
    {
        [NotNull]
        public static ILoggingBuilder AddFluentMigratorSerilog([NotNull] this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.AddSingleton<ILoggerProvider, FluentMigratorSerilogLoggerProvider>();
            return loggingBuilder;
        }
    }
}
