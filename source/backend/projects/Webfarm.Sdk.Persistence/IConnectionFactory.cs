namespace Webfarm.Sdk.Persistence
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConnectionFactory
    {
        /* IDbConnection Open(string connectionString, Dictionary<string, string> properties = null); */

        Task<IDbConnection> OpenAsync(string connectionString, Dictionary<string, string> properties = null, CancellationToken cancellationToken = default);
    }
}
