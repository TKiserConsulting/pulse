using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Webfarm.Sdk.Common.Formatting;
using Webfarm.Sdk.Daemon.Utilities;

namespace Webfarm.Sdk.Persistence.Migrations
{
    public class MigratorApp : MigratorApp<MigrationOptions>
    {
        public MigratorApp([CanBeNull] string applicationName = null)
            : base(applicationName)
        {
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class MigratorApp<T>
#pragma warning restore SA1402 // File may only contain a single type
        where T : MigrationOptions, new()
    {
        private readonly string applicationName;

        public MigratorApp([CanBeNull] string applicationName = null)
        {
            this.applicationName = applicationName;
        }

        public int Run([NotNull]string[] args)
        {
            var assembly1 = Assembly.GetEntryAssembly();
            var assembly2 = this.GetType().Assembly;
            var assemblies = new[] { assembly1, assembly2 }.Where(a => a != null).ToArray();
            return this.Run(_ => assemblies, args);
        }

        public int Run([NotNull]Func<T, Assembly[]> assemblyResolver, [NotNull]string[] args)
        {
            var assembly = Assembly.GetEntryAssembly();
            Contract.Assert(assembly != null);
            var appName = this.applicationName ?? DaemonHelpers.ShortApplicationName(assembly.FullName);
            return this.Run(appName, assemblyResolver, args);
        }

        public int Run([NotNull]string appName, [NotNull]Func<T, Assembly[]> assemblyResolver, [NotNull] string[] args)
        {
            var app = new T
            {
                Name = appName
            };

            app.OnExecute(
                async () =>
                {
                    var result = 0;
                    if (!string.IsNullOrWhiteSpace(app.ConnectionString))
                    {
                        try
                        {
                            await this.Start(app, args, assemblyResolver);
                        }
                        #pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception)
                            #pragma warning restore CA1031 // Do not catch general exception types
                        {
                            result = 2;
                        }
                    }
                    else
                    {
                        app.ShowHint();
                        result = 1;
                    }

                    return result;
                });

            return app.Execute(args);
        }

        private async Task Start([NotNull] T app, [NotNull] string[] args, [NotNull] Func<T, Assembly[]> assemblyResolver)
        {
            Debug.Assert(app != null, nameof(app) + " != null");
            await DaemonHelpers
                .ConfigureAsConsoleDaemon<CompositionRoot>(args)
                .UseRootSerilog(args)
                .ConfigureServices((hb, serviceCollection) =>
                {
                    serviceCollection.AddSingleton<MigrationOptions>(app);
                    this.AddMigrationServices(serviceCollection, app, assemblyResolver);

                    serviceCollection.AddHostedService<MigratorBackgroundService>();
                })
                .UseSerilog((hostBuilderContext, loggerConfiguration) =>
                {
                    var name = hostBuilderContext.HostingEnvironment.ShortApplicationName();
                    loggerConfiguration
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", name);
                    if (app.IsDaemon)
                    {
                        loggerConfiguration.WriteTo.Console(new SpecializedJsonFormatter(suppressMessageTemplate: true));
                    }
                    else
                    {
                       loggerConfiguration.WriteTo.Console();
                    }
                })
                .RunConsoleAsync();
        }

        private void AddMigrationServices([NotNull] IServiceCollection serviceCollection, [NotNull] T app, [NotNull] Func<T, Assembly[]> assemblyResolver)
        {
            var databaseType = app.DatabaseType;
            var connection = this.AppendSchemaToConnectionString(databaseType, app.ConnectionString, app.SchemaName);
            var timeout = app.Timeout;
            var assemblies = assemblyResolver(app);

            serviceCollection
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => this
                    .BuildRunner(rb, databaseType)
                    .WithGlobalConnectionString(connection)
                    .WithGlobalCommandTimeout(timeout)
                    .ScanIn(assemblies).For.All())
                .Configure<RunnerOptions>(opt => opt.Profile = Constants.UpdatablesProfile)
                .Configure<FluentMigratorLoggerOptions>(opt =>
                {
                    opt.ShowSql = true;
                    opt.ShowElapsedTime = false;
                });
        }

        private IMigrationRunnerBuilder BuildRunner(IMigrationRunnerBuilder rb, string databaseType)
        {
            rb = databaseType == Constants.OracleDatabaseType ?
                rb.AddOracleManaged() :
                rb.AddPostgres();

            rb = rb
                .WithRunnerConventions(new DatabaseSpecificMigrationRunnerConventions(databaseType));

            return rb;
        }

        [NotNull]
        private string AppendSchemaToConnectionString([NotNull]string databaseType, [NotNull]string connectionString, [NotNull]string schemaName)
        {
            return !string.IsNullOrEmpty(schemaName) && databaseType == Constants.PostgresqlDatabaseType &&
                   !connectionString.ToUpperInvariant().Contains("search path", StringComparison.OrdinalIgnoreCase)
                ? $"{connectionString};Search Path={schemaName};"
                : connectionString;
        }
    }
}
