namespace Pulse.Persistence.Configuration
{
    using FluentMigrator.Runner.VersionTableInfo;
    using Webfarm.Sdk.Persistence.Migrations;

    [VersionTableMetaData]
    public sealed class VersionTableMetaData : BaseVersionTableMetaData
    {
        public override string SchemaName => PulseDbContext.Schema;
    }
}
