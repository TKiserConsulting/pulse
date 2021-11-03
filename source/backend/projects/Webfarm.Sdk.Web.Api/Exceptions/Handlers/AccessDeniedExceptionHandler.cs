namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System;
    using System.Collections.Specialized;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class AccessDeniedExceptionHandler<T> : OperationErrorBuildExceptionHandler<T>
        where T : Exception
    {
        public AccessDeniedExceptionHandler([NotNull] NameValueCollection collection)
            : base(collection)
        {
        }

        protected override OperationError Build(T exception)
        {
            return OperationErrorExtensions.Build(WellKnownErrorCodeTypes.Forbidden, exception, this.ExceptionPolicy);
        }
    }
}
