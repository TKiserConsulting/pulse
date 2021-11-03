namespace Webfarm.Sdk.Persistence.Migrations.Extensions
{
    using System.Reflection;
    using FluentMigrator.Runner;
    using FluentMigrator.Runner.Initialization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Webfarm.Sdk.Persistence.Configuration;

    public static class ServiceCollectionExtensions
    {
        public const string UpdatablesProfile = "updatables";

        public static void AddMigrations<TService, TAssembly>(
            this IHostBuilder hostBuilder,
            string profile = UpdatablesProfile)
            where TService : CoreMigratorHostedService
        {
            hostBuilder.AddMigrations<TService>(typeof(TAssembly).Assembly, profile);
        }

        public static void AddMigrations<T>(
            this IHostBuilder hostBuilder,
            Assembly scanAssembly,
            string profile = UpdatablesProfile)
            where T : CoreMigratorHostedService
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services
                    .AddMigrations<T>(
                        context.Configuration,
                        scanAssembly,
                        profile);
            });
        }

        public static IServiceCollection AddMigrations<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly scanAssembly,
            string profile = UpdatablesProfile)
            where T : CoreMigratorHostedService
        {
            var options = configuration.Get<ApplicationDatabaseOptions>();
            return services
                .AddFluentMigratorDefaults(
                    options,
                    scanAssembly,
                    profile)
                .AddHostedService<T>();
        }

        public static IServiceCollection AddFluentMigratorDefaults(
            this IServiceCollection services,
            ApplicationDatabaseOptions options,
            Assembly scanAssembly,
            string profile = UpdatablesProfile)
        {
            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithRunnerConventions(new DatabaseSpecificMigrationRunnerConventions(options.DatabaseType.ToString()))
                    .WithGlobalConnectionString(options.DefaultConnectionString)
                    .ScanIn(scanAssembly).For.All())
                .Configure<RunnerOptions>(opt => opt.Profile = profile)
                .Configure<FluentMigratorLoggerOptions>(opt =>
                {
                    opt.ShowSql = true;
                    opt.ShowElapsedTime = true;
                });

            return services;
        }
    }
}
