namespace Pulse.Persistence.Services
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Npgsql;
    using Serilog;
    using Webfarm.Sdk.Persistence.Configuration;

    public abstract class SchemaCoreCreator
    {
        private const string RecreateSchemaKeyName = "RecreateSchema";

        private readonly IOptions<ApplicationDatabaseOptions> options;

        protected SchemaCoreCreator(IOptions<ApplicationDatabaseOptions> options)
        {
            this.options = options;
        }

        protected string SchemaName => this.options.Value.DatabaseSchemaName;

        protected bool RecreateSchema =>
            this.options.Value.DatabaseProperties.ContainsKey(RecreateSchemaKeyName) &&
            !bool.FalseString.Equals(this.options.Value.DatabaseProperties[RecreateSchemaKeyName], StringComparison.OrdinalIgnoreCase);

        protected string DefaultConnectionString => this.options.Value.DefaultConnectionString;

        public void CreateSchema(out bool newSchema)
        {
            var (databaseName, userName) = ParseDatabaseAndUserName(this.DefaultConnectionString);

            var recreateSchema = this.RecreateSchema;

            Log.Information(
                "Using database: {DatabaseName}, schema: {DatabaseSchema}, recreate: {RecreateSchema}",
                databaseName,
                this.SchemaName,
                recreateSchema);

            newSchema = true;

            if (!recreateSchema)
            {
                var schemaExists = this.CheckSchemaExists(this.DefaultConnectionString);

                if (schemaExists)
                {
                    var tablesExist = this.CheckTablesExist(this.DefaultConnectionString);
                    newSchema = !tablesExist;
                }
                else
                {
                    recreateSchema = true;
                }
            }

            if (recreateSchema)
            {
                this.CleanSchema(this.DefaultConnectionString, userName);
            }

            this.ApplyMigrations();
        }

        protected abstract DbContext CreateDbContext();

        private static (string, string) ParseDatabaseAndUserName(string connectionString)
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            return ((string)builder["Database"], (string)builder["User Id"]);
        }

        private static void RunCommand(string connectionString, string sql)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                using (var cmd = new NpgsqlCommand(sql, conn))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                {
                    Log.Logger.Debug("{SQL}", sql.TrimEnd());
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        private static object ExecuteScalar(string connectionString, string sql, NpgsqlParameter[] parameters = default)
        {
            object result;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                using (var cmd = new NpgsqlCommand(sql, conn))
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    Log.Logger.Debug("{SQL}", sql.TrimEnd());
                    result = cmd.ExecuteScalar();
                }

                conn.Close();
            }

            return result;
        }

        private void CleanSchema(string connectionString, string userName)
        {
            // azure cleanup
            userName = userName.Split('@').First();
            var sb = new StringBuilder();
            sb.AppendLine($"DROP SCHEMA IF EXISTS {this.SchemaName} CASCADE;");
            sb.AppendLine($"CREATE SCHEMA {this.SchemaName};");
            sb.AppendLine($"GRANT ALL ON SCHEMA {this.SchemaName} TO {userName};");

            RunCommand(connectionString, sb.ToString());
        }

        private void ApplyMigrations()
        {
            Log.Debug("Update database schema\n");

            using (var dbcontext = this.CreateDbContext())
            {
                dbcontext.Database.Migrate();
            }
        }

        private bool CheckSchemaExists(string connectionString)
        {
            var schemaName = ExecuteScalar(
                connectionString,
                "SELECT schema_name FROM information_schema.schemata WHERE schema_name = @SchemaName;",
                new[] { new NpgsqlParameter("@SchemaName", this.SchemaName) });

            return schemaName != null;
        }

        private bool CheckTablesExist(string connectionString)
        {
            var tableCount = (long)ExecuteScalar(
                connectionString,
                "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @SchemaName;",
                new[] { new NpgsqlParameter("@SchemaName", this.SchemaName) });

            return tableCount > 0;
        }
    }
}
