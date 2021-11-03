namespace Pulse.Persistence.Migrations
{
    using FluentMigrator;
    using Webfarm.Sdk.Persistence.Migrations;
    using Webfarm.Sdk.Persistence.Migrations.Extensions;

    [Profile(ServiceCollectionExtensions.UpdatablesProfile)]
    [PostgresqlDatabaseType]
    public class LocalUpdateStoredProcedures : UpdateStoredProceduresMigration
    {
        public LocalUpdateStoredProcedures()
            : base("Migrations.Updatables")
        {
        }
    }
}
