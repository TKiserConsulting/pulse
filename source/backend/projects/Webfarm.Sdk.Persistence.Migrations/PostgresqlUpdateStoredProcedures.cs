namespace Webfarm.Sdk.Persistence.Migrations
{
    using FluentMigrator;

    [Profile(Constants.UpdatablesProfile)]
    [PostgresqlDatabaseType]
    public class PostgresqlUpdateStoredProcedures : UpdateStoredProceduresMigration
    {
        public PostgresqlUpdateStoredProcedures(string nsp)
            : base(nsp)
        {
        }

        public PostgresqlUpdateStoredProcedures()
            : this(Constants.PostgresqlNamespace)
        {
        }
    }
}
