namespace Webfarm.Sdk.Persistence.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentMigrator;
    using FluentMigrator.Infrastructure;
    using FluentMigrator.Runner;
    using FluentMigrator.Runner.Infrastructure;
    using JetBrains.Annotations;

    public class DatabaseSpecificMigrationRunnerConventions : IMigrationRunnerConventions
    {
        private readonly string databaseType;

        public DatabaseSpecificMigrationRunnerConventions(string databaseType)
        {
            this.databaseType = databaseType;
            this.TypeIsMigration = this.TypeIsMigrationImpl;
            this.TypeIsProfile = this.TypeIsProfileImpl;
        }

        public Func<Type, bool> TypeIsMigration { get; }

        public Func<Type, bool> TypeIsProfile { get; }

        public Func<Type, MigrationStage?> GetMaintenanceStage => DefaultMigrationRunnerConventions.Instance.GetMaintenanceStage;

        public Func<Type, bool> TypeIsVersionTableMetaData => DefaultMigrationRunnerConventions.Instance.TypeIsVersionTableMetaData;

        [Obsolete("Have to implement interface, but delegated method is obsolete")]
        public Func<Type, IMigrationInfo> GetMigrationInfo => DefaultMigrationRunnerConventions.Instance.GetMigrationInfo;

        public Func<IMigration, IMigrationInfo> GetMigrationInfoForMigration => DefaultMigrationRunnerConventions.Instance.GetMigrationInfoForMigration;

        public Func<Type, bool> TypeHasTags => DefaultMigrationRunnerConventions.Instance.TypeHasTags;

        public Func<Type, IEnumerable<string>, bool> TypeHasMatchingTags => DefaultMigrationRunnerConventions.Instance.TypeHasMatchingTags;

        private bool TypeIsMigrationImpl(Type type)
        {
            return DefaultMigrationRunnerConventions.Instance.TypeIsMigration(type) && this.TypeIsKnownMigrationImpl(type);
        }

        private bool TypeIsProfileImpl(Type type)
        {
            return DefaultMigrationRunnerConventions.Instance.TypeIsProfile(type) && this.TypeIsKnownMigrationImpl(type);
        }

        private bool TypeIsKnownMigrationImpl([NotNull] Type type)
        {
            var isValid = type
                   .GetCustomAttributes(true)
                   .OfType<DatabaseTypeAttribute>()
                   .Any(attr => string.Equals(this.databaseType, attr.Name, StringComparison.OrdinalIgnoreCase));

            return isValid;
        }
    }
}
