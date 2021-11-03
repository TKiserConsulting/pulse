namespace Webfarm.Sdk.Web.Api.Data
{
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Routing;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class OperationErrorResult : ObjectResult
    {
        public OperationErrorResult(
            [NotNull] OperationError operationError)
            : base(operationError)
        {
            Contract.Assert(operationError != null);
            this.StatusCode = (int)(operationError.Status ?? HttpStatusCode.BadRequest);
            this.ContentTypes.Add(OperationError.ErrorContentType);
        }

        [NotNull]
        public static OperationErrorResult FromModelState(
            ModelStateDictionary modelState,
            [CanBeNull] HttpContext context,
            IncludeErrorDetailPolicyType exceptionPolicy)
        {
            var operationError = new OperationError()
                .With(WellKnownErrorCodeTypes.InvalidData, instance: context?.Request?.Path)
                .With(modelState, exceptionPolicy);
            return new OperationErrorResult(operationError);
        }

        public async Task ExecuteResultAsync(HttpContext httpContext)
        {
            var actionCtx = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            await this.ExecuteResultAsync(actionCtx);
        }
    }
}
