namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System;
    using System.Collections.Specialized;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class UnexpectedExceptionHandler : OperationErrorBuildExceptionHandler<Exception>
    {
        public UnexpectedExceptionHandler([NotNull] NameValueCollection collection)
            : base(collection)
        {
        }

        protected override OperationError CreateOperationError([NotNull] Exception exception, Guid handlingInstanceId)
        {
            var operationError = base.CreateOperationError(exception, handlingInstanceId);
            operationError.Title = this.FormatErrorMessage(exception, handlingInstanceId);
            return operationError;
        }

        protected override OperationError Build(Exception exception)
        {
            return OperationErrorExtensions.Build(WellKnownErrorCodeTypes.UnexpectedError, exception, this.ExceptionPolicy);
        }
    }
}
