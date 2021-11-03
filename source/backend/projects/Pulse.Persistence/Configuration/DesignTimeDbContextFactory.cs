namespace Pulse.Persistence.Configuration
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Persistence;
    using Webfarm.Sdk.Data.Exceptions;

    /// <summary>
    /// required while adding migrations on dev machine
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PulseDbContext>
    {
        private const string ConnectionStringEnvironmentVariableName = "Pulse_ConnectionStrings__Default";

        private static string DefaultConnectionString
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariableName);
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ExecutionException(
                        $"Could not read ConnectionString from environment variable '{ConnectionStringEnvironmentVariableName}'");
                }

                return connectionString;
            }
        }

        public PulseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PulseDbContext>()
                .UseNpgsql(
                    DefaultConnectionString,
                    opt => opt.SetPostgresVersion(10, 11));

            return new PulseDbContext(optionsBuilder.Options);
        }
    }
}
