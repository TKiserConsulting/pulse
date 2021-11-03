namespace Webfarm.Sdk.Persistence.Impl
{
    using Webfarm.Sdk.Persistence.Configuration;

    public class DefaultConnectionStringProvider : IConnectionStringProvider
    {
        private readonly ApplicationDatabaseOptions options;

        public DefaultConnectionStringProvider(ApplicationDatabaseOptions options)
        {
            this.options = options;
        }

        public string LoadConnectionString()
        {
            return this.options.DefaultConnectionString;
        }
    }
}
