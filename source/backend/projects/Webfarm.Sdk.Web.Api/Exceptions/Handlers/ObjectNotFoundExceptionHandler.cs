namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class ObjectNotFoundExceptionHandler : OperationErrorBuildExceptionHandler<ObjectNotFoundException>
    {
        public ObjectNotFoundExceptionHandler([NotNull] NameValueCollection collection)
            : base(collection)
        {
        }

        [NotNull]
        protected override OperationError Build([NotNull] ObjectNotFoundException exception)
        {
            Contract.Assert(exception != null);

            var operationError = OperationErrorExtensions.Build(WellKnownErrorCodeTypes.ResourceNotFound, exception, this.ExceptionPolicy);

            operationError.AddData("ResourceId", exception.ObjectId);
            operationError.AddData("ResourceType", exception.ObjectType);
            return operationError;
        }
    }
}
