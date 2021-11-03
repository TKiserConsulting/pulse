namespace Webfarm.Sdk.Persistence.Migrations
{
    public sealed class PostgresqlDatabaseTypeAttribute : DatabaseTypeAttribute
    {
        public const string PostgresqlDatabaseType = "postgres";

        public PostgresqlDatabaseTypeAttribute()
            : base(PostgresqlDatabaseType)
        {
        }
    }
}
