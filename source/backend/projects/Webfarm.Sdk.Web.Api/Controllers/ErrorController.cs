namespace Infoplus.Askod.Portal.Web.Api.Controllers
{
    using Infoplus.Askod.Sdk.Data.Metadata;
    using Infoplus.Askod.Sdk.Web.Api.Data;
    using Infoplus.Askod.Sdk.Web.Api.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NSwag.Annotations;

    // todo [ds]: required for portal but breaks oauth ((
    /*
    [Route("{*url}", Order = 999)]
    [OpenApiIgnore]
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpHead]
        [HttpOptions]
        [AcceptVerbs("PATCH")]
        public IActionResult Handle404()
        {
            var operationError = new OperationError()
                .With(BasicErrorCodeTypes.ResourceNotFound, instance: this.Request.Path);
            var result = new OperationErrorResult(operationError);
            return result;
        }
    }
    */
}
