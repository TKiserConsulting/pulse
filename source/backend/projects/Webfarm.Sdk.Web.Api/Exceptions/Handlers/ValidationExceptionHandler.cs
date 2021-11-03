namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class ValidationExceptionHandler : OperationErrorBuildExceptionHandler<ApplicationValidationException>
    {
        public ValidationExceptionHandler([NotNull] NameValueCollection collection)
            : base(collection)
        {
        }

        [NotNull]
        protected override OperationError Build([NotNull] ApplicationValidationException exception)
        {
            Contract.Assert(exception != null);

            var operationError = new OperationError().WithValidation(exception, this.ExceptionPolicy);
            return operationError;
        }
    }
}
