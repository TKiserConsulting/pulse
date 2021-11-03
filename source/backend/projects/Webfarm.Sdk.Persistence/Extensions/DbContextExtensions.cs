namespace Webfarm.Sdk.Persistence.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;

    using static System.Text.RegularExpressions.Regex;

    public static class DbContextExtensions
    {
        public static void UseSingularTableNames(this DbContext dbcontext, ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(DisplayName(entityType));
            }
        }

        public static string DisplayName(IMutableEntityType entityType)
        {
            return entityType.DisplayName().ReplaceGenericSuffix();
        }

        private static string ReplaceGenericSuffix(this string name)
        {
            // EF has generic models (e.g. IdentityUserClaim<string>), ShortDisplayName() will generate name with generics. remove them
            var startIndex = name.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);
            return startIndex > 0 ? name.Substring(0, startIndex) : name;
        }

        public static void UseSnakeCase(this DbContext dbcontext, ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName().ToSnakeCase();
                entity.SetTableName(tableName);

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
                }
            }
        }

        public static async Task Reset(this DbContext context, CancellationToken cancellationToken)
        {
            var entries = context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToArray();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        await entry.ReloadAsync(cancellationToken);
                        break;
                }
            }
        }

        private static string ToSnakeCase(this string input)
        {
            var result = input;

            if (!string.IsNullOrEmpty(input))
            {
                var startUnderscores = Match(input, @"^_+");
                result = startUnderscores + Replace(input, $@"([a-z0-9])([A-Z])", $"$1_$2").ToLower(CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}
