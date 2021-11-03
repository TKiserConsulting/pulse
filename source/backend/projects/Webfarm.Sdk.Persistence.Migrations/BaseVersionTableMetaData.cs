namespace Webfarm.Sdk.Persistence.Migrations
{
    using FluentMigrator.Runner.VersionTableInfo;
    using JetBrains.Annotations;

    [VersionTableMetaData]
    public abstract class BaseVersionTableMetaData : IVersionTableMetaData
    {
        [NotNull]
        public abstract string SchemaName { get; }

        public virtual bool OwnsSchema => true;

        // ReSharper disable once StringLiteralTypo
        [NotNull]
        public string TableName => "__migrations_b";

        [NotNull]
        public string ColumnName => "version";

        [NotNull]
        public string AppliedOnColumnName => "applied_on";

        [NotNull]
        public string DescriptionColumnName => "description";

        // ReSharper disable once StringLiteralTypo
        [NotNull]
        public string UniqueIndexName => "uk_version";

        public object ApplicationContext { get; set; }
    }
}
