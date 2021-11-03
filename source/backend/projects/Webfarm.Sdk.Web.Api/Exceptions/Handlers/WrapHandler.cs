namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Webfarm.Sdk.Web.Api.Data;

    public class WrapHandler<T> : IExceptionHandler
        where T : Exception
    {
        protected internal const string MessageKey = "Message";

        protected internal const string AppendHandlingIdKey = "AppendHandlingId";

        protected internal const string PolicyKey = "IncludeErrorDetailPolicy";

        private readonly string message;

        private readonly bool appendHandlingId;

        public WrapHandler([NotNull] NameValueCollection collection)
        {
            Contract.Assert(collection != null);

            if (collection.AllKeys.Contains(MessageKey))
            {
                this.message = collection[MessageKey];
            }

            if (collection.AllKeys.Contains(AppendHandlingIdKey))
            {
                this.appendHandlingId = bool.Parse(collection[AppendHandlingIdKey]);
            }

            if (collection.AllKeys.Contains(PolicyKey))
            {
                this.ExceptionPolicy = (IncludeErrorDetailPolicyType)Enum.Parse(typeof(IncludeErrorDetailPolicyType), collection[PolicyKey]);
            }
        }

        protected IncludeErrorDetailPolicyType ExceptionPolicy { get; } = IncludeErrorDetailPolicyType.Omit;

        [NotNull]
        public virtual Exception HandleException([NotNull] Exception exception, Guid handlingInstanceId)
        {
            var error = this.FormatErrorMessage(exception, handlingInstanceId);
            var result = (T)Activator.CreateInstance(typeof(T), error, exception);
            return result;
        }

        protected virtual string FormatErrorMessage([NotNull] Exception exception, Guid handlingInstanceId)
        {
            Contract.Assert(exception != null);

            var pattern = this.message ?? exception.Message;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this.appendHandlingId && (pattern == null || !pattern.Contains("{handlingInstanceID}", StringComparison.OrdinalIgnoreCase)))
            {
                pattern = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}. [Error ID: {{handlingInstanceID}}]",
                    pattern?.Trim(' ', '.'));
            }

            var error = ExceptionUtility.FormatExceptionMessage(pattern, handlingInstanceId);
            return error;
        }
    }
}
