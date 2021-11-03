namespace Webfarm.Sdk.Persistence.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Npgsql;
    using Webfarm.Sdk.Persistence.Configuration;

    public class PostgresConnectionFactory : IConnectionFactory
    {
        [ItemNotNull]
        public async Task<IDbConnection> OpenAsync(string connectionString, [CanBeNull] Dictionary<string, string> properties = null, CancellationToken cancellationToken = default)
        {
            NpgsqlConnection connection = null;
            try
            {
                if (properties != null
                    && properties.ContainsKey(ApplicationDatabaseProperties.Schema)
                    && !string.IsNullOrEmpty(properties[ApplicationDatabaseProperties.Schema])
                    && !string.IsNullOrEmpty(connectionString)
                    && !connectionString.ToUpperInvariant().Contains("SEARCH PATH", StringComparison.InvariantCulture))
                {
                    connectionString = $"{connectionString};Search Path={properties[ApplicationDatabaseProperties.Schema]};";
                }

                connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                return connection;
            }
            catch (Exception)
            {
                connection?.Dispose();
                throw;
            }
        }
    }
}
