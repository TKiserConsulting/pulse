namespace Infoplus.Askod.Portal.Web.Api.Common.Authorization
{
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;

    internal class GrantAuthorizationRequirement : IAuthorizationRequirement
    {
        public GrantAuthorizationRequirement([CanBeNull] string grantType, [CanBeNull] string grantAction)
        {
            this.GrantType = string.IsNullOrEmpty(grantType) ? "default" : grantType;
            this.GrantAction = string.IsNullOrEmpty(grantAction) ? "read" : grantAction;
        }

        public string GrantType { get; }

        public string GrantAction { get; }
    }
}
