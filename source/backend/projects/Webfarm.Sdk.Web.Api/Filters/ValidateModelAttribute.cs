namespace Infoplus.Askod.Web.Core.Api.Filters
{
    using System;
    using System.Linq;

    using Infoplus.Askod.Data.Metadata;
    using Infoplus.Askod.Web.Core.Api.Data;
    using Infoplus.Askod.Web.Core.Api.Extensions;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var operationError = new OperationError()
                    .With(ErrorCodeTypes.InvalidData);

                var items =
                    from entry in context.ModelState
                    from error in entry.Value.Errors
                    select Tuple.Create(entry.Key, error.ErrorMessage, error.Exception);
                items.ForEach(item => operationError.AddError(item.Item1, string.Empty, item.Item2, TODO, item.Item3));

                context.Result = new BadRequestObjectResult(operationError)
                {
                    StatusCode = (int)operationError.Status
                };
            }
        }
    }
}
