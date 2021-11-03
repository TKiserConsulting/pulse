namespace Pulse.Api.BareboneControllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Webfarm.Sdk.Common.Authorization;
    using Webfarm.Sdk.Data.ComponentModel;

    [GrantAuthorizationSkip]
    [Route("[controller]")]
    public class AclController : ControllerBase
    {
        private readonly IGrantManager grantManager;

        public AclController(IGrantManager grantManager)
        {
            this.grantManager = grantManager;
        }

        [NotNull]
        [HttpGet]
        [Route("check")]
        public Task<GrantResult> CheckSingle(
            [BindRequired] [NotNull] string grantType,
            [BindRequired] [NotNull] string grantAction,
            CancellationToken cancellationToken)
        {
            var grants = new[]
            {
                new GrantDescriptor
                {
                    Type = grantType,
                    Action = grantAction
                }
            };

            return this.CheckMany(grants, cancellationToken);
        }

        [NotNull]
        [HttpPost]
        [Route("check-many")]
        [GrantAction("read")]
        public Task<GrantResult> CheckMany(
            [BindRequired] [NotNull] GrantDescriptor[] grants,
            CancellationToken cancellationToken)
        {
            var input = new GrantInput
            {
                Principal = this.User,
                Grants = grants
            };

            return this.grantManager.ValidateGrants(input, cancellationToken);
        }
    }
}
