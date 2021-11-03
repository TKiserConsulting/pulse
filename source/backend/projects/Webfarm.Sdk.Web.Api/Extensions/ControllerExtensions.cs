namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Exceptions;

    public static class ControllerExtensions
    {
        public static CreatedResult Created(
            [NotNull] this ControllerBase controller,
            object result,
            string routeName,
            object values)
        {
            Contract.Assert(controller != null);
            var address = controller.Url.RouteUrl(routeName, values);
            var uri = new Uri(address, UriKind.Relative);
            return controller.Created(uri, result);
        }

        public static void ApplyPatch<T>([NotNull] this ControllerBase controller, [NotNull] JsonPatchDocument<T> patchDoc, T model)
            where T : class
        {
            Contract.Assert(controller != null);

            patchDoc.ApplyTo(model, error =>
            {
                Log.Logger.Warning($"Error while patching model. Operation:{error.Operation}, ErrorMessage: {error.ErrorMessage}");
            });
            if (!controller.ModelState.IsValid)
            {
                var operationError = new OperationError()
                    .With(WellKnownErrorCodeTypes.InvalidData)
                    .With(controller.ModelState, IncludeErrorDetailPolicyType.Omit);
                throw new OperationErrorApiException(operationError);
            }
        }
    }
}
