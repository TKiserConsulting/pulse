using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Webfarm.Sdk.Data.Metadata;
using Webfarm.Sdk.Web.Api.Data;
using Webfarm.Sdk.Web.Api.Extensions;

namespace Webfarm.Sdk.Web.Api.Authorization
{
    [Obsolete("not used")]
    public class PermissionAuthorizationHandler : AttributeAuthorizationHandler<PermissionAuthorizationRequirement, PermissionAttribute>
    {
        protected override async Task HandleRequirementAsync([NotNull] AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement, [NotNull] IEnumerable<PermissionAttribute> attributes)
        {
            Contract.Assert(context != null);
            Debug.Assert(context.User != null, "context.User != null");
            Contract.Assert(attributes != null);

            foreach (var permissionAttribute in attributes)
            {
                if (!await AuthorizeAsync(context.User, permissionAttribute.Name))
                {
                    HandleAccessDenied(context, requirement);
                    return;
                }
            }

            context.Succeed(requirement);
        }

        private static Task<bool> AuthorizeAsync([NotNull] ClaimsPrincipal user, string permission)
        {
            var hasPermission = user.HasClaim(cl => cl.Type == permission);
            return Task.FromResult(hasPermission);
        }

        private static void HandleAccessDenied(
            [NotNull] AuthorizationHandlerContext context,
            PermissionAuthorizationRequirement requirement)
        {
            var authorizationFilterContext = context.Resource as AuthorizationFilterContext;
            var error = new OperationError().With(BasicErrorCodeTypes.Forbidden);
            if (authorizationFilterContext != null && error.Status != null)
            {
                authorizationFilterContext.Result = new JsonResult(error) { StatusCode = (int)error.Status.Value };

                // wait for implementation of: https://github.com/aspnet/Security/issues/1359
                // hack: https://github.com/aspnet/Security/issues/1560
                context.Succeed(requirement);
            }
        }
    }
}
