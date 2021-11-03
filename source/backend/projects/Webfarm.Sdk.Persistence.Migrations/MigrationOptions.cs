
namespace Webfarm.Sdk.Persistence.Migrations
{
    using System;

    public class MigrationOptions
    {
        private const int DefaultConnectionTimeout = 1200;

        public MigrationOptions()
        {
            this.Timeout = TimeSpan.FromSeconds(DefaultConnectionTimeout);
        }

        public string ConnectionString { get; set; }

        public string DatabaseType { get; set; }

        public string SchemaName { get; set; }

        public TimeSpan Timeout
        {
            get;
            set;
        }
    }
}
