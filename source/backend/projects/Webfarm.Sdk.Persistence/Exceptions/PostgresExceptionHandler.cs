namespace Webfarm.Sdk.Persistence.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Npgsql;
    using Webfarm.Sdk.Data.Metadata;

    #pragma warning disable CA1303 // Do not pass literals as localized parameters
    public class PostgresExceptionHandler : IExceptionHandler
    {
        public static readonly IDictionary<string, string> ErrorSubCodeMapping = new Dictionary<string, string>
        {
            { "23505", WellKnownErrorSubcodeTypes.Duplicated },
            { "23503", WellKnownErrorSubcodeTypes.Restricted }
        };

        // ReSharper disable once UnusedParameter.Local
        #pragma warning disable CA1801
        #pragma warning disable IDE0060
        public PostgresExceptionHandler(NameValueCollection collection)
        {
        }

        [NotNull]
        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            return this.ConvertException(exception);
        }

        [CanBeNull]
        protected static string GetErrorSubCode([NotNull] PostgresException exception)
        {
            Debug.Assert(exception != null, "exception != null");
            var number = exception.SqlState;
            var subCode = WellKnownErrorSubcodeTypes.Default;
            if (ErrorSubCodeMapping.ContainsKey(number))
            {
                subCode = ErrorSubCodeMapping[number];
            }

            return subCode;
        }

        [NotNull]
        protected Exception ConvertException(Exception exception)
        {
            Exception result = null;
            var postgresException = this.FindPostgresException(exception);
            if (postgresException != null)
            {
                result = this.ConvertDatabaseException(postgresException);
            }

            result = result ?? new PersistenceException("Unexpected persistence exception", exception) { DatabaseType = DatabaseType.Postgres };
            return result;
        }

        protected PostgresException FindPostgresException(Exception exception)
        {
            PostgresException ex = null;
            if (exception is PostgresException postgresException)
            {
                ex = postgresException;
            }
            else if (exception.InnerException != null)
            {
                ex = this.FindPostgresException(exception.InnerException);
            }

            return ex;
        }

        [NotNull]
        protected virtual Exception ConvertDatabaseException([NotNull] PostgresException exception)
        {
            Debug.Assert(exception != null, "registration != null");
            Exception result;
            switch (exception.SqlState)
            {
                // case "23000" /*integrity_constraint_violation*/:
                // case "23001" /*restrict_violation*/:
                // case "23502" /*not_null_violation*/:
                // case "23514" /*check_violation*/:
                // case "23P01" /*exclusion_violation*/:
                case "23503" /*foreign_key_violation*/:
                case "23505" /*unique_violation*/:
                    result = new PersistenceConstraintException(exception)
                    {
                        ErrorSubcode = GetErrorSubCode(exception),
                        TableName = exception.TableName,
                        ConstraintName = exception.ConstraintName,
                        DatabaseType = DatabaseType.Postgres
                    };
                    break;
                default:
                    result = new PersistenceException("Operation cannot be finished", exception)
                    {
                        ErrorSubcode = GetErrorSubCode(exception),
                        DatabaseType = DatabaseType.Postgres
                    };
                    break;
            }

            return result;
        }
    }
}
