namespace Webfarm.Sdk.Persistence.Migrations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentMigrator.Runner;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class CoreMigratorHostedService : IHostedService
    {
        private readonly IServiceProvider rootServiceProvider;
        private readonly ILogger logger;

        public CoreMigratorHostedService(
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            this.rootServiceProvider = serviceProvider;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        [NotNull]
        protected Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.logger.LogTrace("Start migration");

                this.Migrate(stoppingToken);

                this.logger.LogTrace("Migration finished");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unexpected error while running migrations");
                throw;
            }
        }

        protected virtual void Migrate(CancellationToken stoppingToken)
        {
            using (var scope = this.rootServiceProvider.CreateScope())
            {
                this.Migrate(scope, stoppingToken);
            }
        }

        protected virtual void Migrate(IServiceScope scope, CancellationToken stoppingToken)
        {
            this.Migrate(scope.ServiceProvider, stoppingToken);
        }

        protected virtual void Migrate(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            this.Migrate(runner, stoppingToken);
        }

        protected virtual void Migrate([NotNull] IMigrationRunner runner, CancellationToken stoppingToken)
        {
            runner.MigrateUp();
        }
    }
}
