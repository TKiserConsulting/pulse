namespace Webfarm.Sdk.Persistence
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class ConnectionHealthCheck : IHealthCheck
    {
        public static readonly string Key = "connection";

        private readonly IConnectionFactory connectionFactory;

        private readonly Lazy<string> connectionString;

        public ConnectionHealthCheck([NotNull] IConnectionFactory connectionFactory, [NotNull] IConnectionStringProvider connectionStringProvider)
        {
            Contract.Assert(connectionFactory != null);
            Contract.Assert(connectionStringProvider != null);
            this.connectionFactory = connectionFactory;
            this.connectionString = new Lazy<string>(connectionStringProvider.LoadConnectionString);
        }

        public virtual string Name => Key;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult result;
            try
            {
                using (var connection = await this.connectionFactory.OpenAsync(this.connectionString.Value, cancellationToken: cancellationToken))
                {
                    result = HealthCheckResult.Healthy();
                    connection.Close();
                }
            }
            #pragma warning disable CA1031 // Do not catch general exception types
            catch
            #pragma warning restore CA1031 // Do not catch general exception types
            {
                result = HealthCheckResult.Unhealthy();
            }

            return result;
        }
    }
}
