// ReSharper disable CA1308
namespace Webfarm.Sdk.Web.Api.Authorization
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Serilog;
    using Webfarm.Sdk.Common.Authorization;

    public class ContractBasedGrantAuthorizationHandler
        : AuthorizationHandler<ContractBasedGrantAuthorizationRequirement>
    {
        private readonly ILogger logger;
        private readonly IGrantManager grantManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContractBasedGrantAuthorizationHandler(ILogger logger, IGrantManager grantManager, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.grantManager = grantManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        [NotNull]
        protected override async Task HandleRequirementAsync(
            [NotNull] AuthorizationHandlerContext context,
            ContractBasedGrantAuthorizationRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var resource = this.httpContextAccessor?.HttpContext?.GetEndpoint();
                var grantContext = new GrantContext(resource);
                if (grantContext.SkipAuthorization)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    this.logger.Debug("Resolving grant: [{GrantType},{GrantAction}]", grantContext.GrantType, grantContext.GrantAction);

                    var grantInput = new GrantInput
                    {
                        Principal = context.User,
                        Grants = new[]
                        {
                            new GrantDescriptor
                            {
                                Type = grantContext.GrantType,
                                Action = grantContext.GrantAction
                            }
                        }
                    };

                    Contract.Assert(this.grantManager != null, "this.grantManager != null");
                    var result = await this.grantManager.ValidateGrants(grantInput);
                    if (result.Allow)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
        }
    }
}
