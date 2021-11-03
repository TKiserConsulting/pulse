namespace Webfarm.Sdk.Persistence.Configuration
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;

    public class ApplicationDatabaseOptions
    {
        private string databaseSchemaName;

        public ApplicationDatabaseOptions()
        {
            this.DefaultConnectionName = "Default";
            this.DatabaseType = DatabaseType.Postgres;
            this.DatabaseProperties = new Dictionary<string, string>();
        }

        public DatabaseType DatabaseType
        {
            get;
            set;
        }

        public string DefaultConnectionName
        {
            get;
            set;
        }

        public Dictionary<string, string> ConnectionStrings
        {
            get;
            set;
        }

        public Dictionary<string, string> DatabaseProperties
        {
            get;
            set;
        }

        public string DatabaseSchemaName
        {
            get => this.databaseSchemaName;
            set
            {
                this.DatabaseProperties[ApplicationDatabaseProperties.Schema] = value;
                this.databaseSchemaName = value;
            }
        }

        public string DefaultConnectionString => this.ConnectionStrings[this.DefaultConnectionName];

        [NotNull]
        public ApplicationDatabaseOptions SetDatabaseProperty([NotNull] string key, string value)
        {
            this.DatabaseProperties[key] = value;
            return this;
        }
    }
}
