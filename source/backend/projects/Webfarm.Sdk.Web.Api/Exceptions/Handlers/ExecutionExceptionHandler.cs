namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class ExecutionExceptionHandler : OperationErrorBuildExceptionHandler<ExecutionException>
    {
        public ExecutionExceptionHandler([NotNull] NameValueCollection collection)
            : base(collection)
        {
        }

        protected override OperationError Build([NotNull] ExecutionException exception)
        {
            Contract.Assert(exception != null);

            var result =
                exception is OperationErrorApiException operationErrorApiException
                ? operationErrorApiException.Error
                : OperationErrorExtensions.Build(
                    WellKnownErrorCodeTypes.OperationCannotBeFinished,
                    exception,
                    this.ExceptionPolicy,
                    exception.ErrorCode);

            return result;
        }
    }
}
