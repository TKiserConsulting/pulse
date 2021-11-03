namespace Webfarm.Sdk.Persistence.Migrations
{
    public sealed class OracleDatabaseTypeAttribute : DatabaseTypeAttribute
    {
        public OracleDatabaseTypeAttribute()
            : base(Constants.OracleDatabaseType)
        {
        }
    }
}
