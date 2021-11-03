namespace Pulse.Api
{
    using System.Threading.Tasks;
    using Configuration;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Hosting;
    using Persistence.Configuration;
    using Persistence.Services;
    using Webfarm.Sdk.Daemon.Utilities;
    using Webfarm.Sdk.Persistence.Migrations.Extensions;

    public static class Program
    {
        [NotNull]
        public static Task<int> Main([NotNull] string[] args)
        {
            return DaemonHelpers
                .RunAsConsoleDaemonAsync<CompositionRoot, Startup>(args, AddMigrations);
        }

        private static void AddMigrations(IHostBuilder hostBuilder)
        {
            hostBuilder.AddMigrations<MigrationService, VersionTableMetaData>();
        }
    }
}
