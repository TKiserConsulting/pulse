namespace Infoplus.Askod.Sdk.Daemon.Migrations.Logging
{
    using System;
    using FluentMigrator.Runner;
    using FluentMigrator.Runner.Logging;
    using JetBrains.Annotations;
    using Serilog;

    public class FluentMigratorSerilogLogger : FluentMigratorLogger
    {
        // do not want to have context information in the log for now
        // because only one context used
        // private static readonly ILogger Logger = Serilog.Log.ForContext<FluentMigratorLogger>();
        private static readonly ILogger Logger = Serilog.Log.Logger;

        public FluentMigratorSerilogLogger(FluentMigratorLoggerOptions options)
            : base(options)
        {
        }

        protected override void WriteError(Exception exception)
        {
            Logger.Error(exception, "Migration error");
        }

        protected override void WriteHeading(string message)
        {
            Logger.Information("{heading}", message);
        }

        protected override void WriteEmphasize(string message)
        {
            Logger.Information("[+] {emphasize}", message);
        }

        protected override void WriteSql([CanBeNull] string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                this.WriteEmptySql();
            }
            else
            {
                Logger.Debug("{sql}", sql);
            }
        }

        protected override void WriteEmptySql()
        {
            Logger.Debug("No SQL statement executed.");
        }

        protected override void WriteSay(string message)
        {
            Logger.Verbose("{say}", message);
        }

        protected override void WriteElapsedTime(TimeSpan timeSpan)
        {
            Logger.Information("=> {TotalSeconds}s", timeSpan.TotalSeconds);
        }

        protected override void WriteError(string message)
        {
            Logger.Error("{error}", message);
        }
    }
}
