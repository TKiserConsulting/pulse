namespace Pulse.Persistence.Services
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Webfarm.Sdk.Persistence.Configuration;

    public class DatabaseCreator : SchemaCoreCreator
    {
        private readonly ILoggerFactory loggerFactory;

        public DatabaseCreator(IServiceProvider serviceProvider)
            :this(
                serviceProvider.GetService<ILoggerFactory>(),
                serviceProvider.GetService<IOptions<ApplicationDatabaseOptions>>())
        {
        }

        public DatabaseCreator(
            ILoggerFactory loggerFactory,
            IOptions<ApplicationDatabaseOptions> options)
            : base(options)
        {
            this.loggerFactory = loggerFactory;
        }

        protected override DbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PulseDbContext>()
                .UseNpgsql(
                    this.DefaultConnectionString,
                    opt => opt
                        .MigrationsHistoryTable("__migrations_a", this.SchemaName)
                        .SetPostgresVersion(10, 11));

            return new PulseDbContext(optionsBuilder.Options, this.loggerFactory);
        }
    }
}
