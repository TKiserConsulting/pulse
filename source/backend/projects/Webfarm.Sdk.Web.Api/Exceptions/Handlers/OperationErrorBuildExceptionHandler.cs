namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Web.Api.Data;

    public abstract class OperationErrorBuildExceptionHandler<TType> : WrapHandler<TType>
        where TType : Exception
    {
        protected OperationErrorBuildExceptionHandler([NotNull] NameValueCollection collection)
            : base(collection)
        {
        }

        public override Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            var incomeException = exception as TType;
            Contract.Assert(incomeException != null, typeof(TType).Name + " is expected");
            var operationError = this.CreateOperationError(incomeException, handlingInstanceId);
            var result = new OperationErrorApiException(operationError);
            return result;
        }

        [NotNull]
        protected virtual OperationError CreateOperationError(TType exception, Guid handlingInstanceId)
        {
            var operationError = this.Build(exception);
            operationError.HandlingInstanceId = handlingInstanceId;
            return operationError;
        }

        protected abstract OperationError Build(TType exception);
    }
}
