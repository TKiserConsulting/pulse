namespace Webfarm.Sdk.Web.Api.Providers
{
    using System.Diagnostics.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class ApiVersioningResponseProvider : DefaultErrorResponseProvider
    {
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            Contract.Assert(context != null);

            IActionResult result;

            switch (context.ErrorCode)
            {
                case ErrorCodes.UnsupportedApiVersion:
                case ErrorCodes.InvalidApiVersion:
                case ErrorCodes.AmbiguousApiVersion:
                case ErrorCodes.ApiVersionUnspecified:
                    var error = new OperationError
                        {
                            Detail = context.Message
                        }
                        .With(context.ErrorCode);
                    result = new JsonResult(error);
                    break;
                default:
                    result = base.CreateResponse(context);
                    break;
            }

            return result;
        }
    }
}
