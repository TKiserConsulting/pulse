// ReSharper disable CA1308
namespace Infoplus.Askod.Portal.Web.Api.Common.Authorization
{
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;

    internal class GrantAuthorizationHandler : AuthorizationHandler<GrantAuthorizationRequirement>
    {
        [NotNull]
        protected override Task HandleRequirementAsync(
            [NotNull] AuthorizationHandlerContext context,
            [NotNull] GrantAuthorizationRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // todo [ds]: call OPA
                var granted = requirement.GrantAction == "read";
                if (granted)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}
