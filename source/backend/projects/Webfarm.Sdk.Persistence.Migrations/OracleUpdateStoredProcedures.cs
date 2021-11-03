using FluentMigrator;

namespace Webfarm.Sdk.Persistence.Migrations
{
    [Profile(Constants.UpdatablesProfile)]
    [OracleDatabaseType]
    public class OracleUpdateStoredProcedures : UpdateStoredProceduresMigration
    {
        public OracleUpdateStoredProcedures(string nsp)
            : base(nsp)
        {
        }

        public OracleUpdateStoredProcedures()
            : this(Constants.OracleNamespace)
        {
        }
    }
}
