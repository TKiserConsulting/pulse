namespace Infoplus.Askod.Portal.Web.Api.Common.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;

    internal class GrantAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public GrantAuthorizationPolicyProvider([NotNull] IOptions<AuthorizationOptions> options)
        {
            // new DefaultAuthorizationPolicyProvider();
            /*
            options.Value.DefaultPolicy =
                AuthorizationPolicy.Combine(
                    options.Value.DefaultPolicy,
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddRequirements(new ContractBasedGrantAuthorizationRequirement())
                        .Build());
            */
            this.BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            var r = await this.BackupPolicyProvider.GetDefaultPolicyAsync();
            return r;
        }

        public async Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            var r = await this.BackupPolicyProvider.GetFallbackPolicyAsync();
            return r;
        }

        [NotNull]
        public async Task<AuthorizationPolicy> GetPolicyAsync([NotNull] string policyName)
        {
            // Task<AuthorizationPolicy> result;
            AuthorizationPolicy result;
            /*if (policyName.StartsWith(GrantAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme);
                var parts = policyName.Split(":").Skip(1).ToArray();
                var grantType = parts.Skip(0).FirstOrDefault();
                var grantAction = parts.Skip(1).FirstOrDefault();
                policy
                    .RequireAuthenticatedUser()
                    .AddRequirements(new GrantAuthorizationRequirement(grantType, grantAction));
                result = Task.FromResult(policy.Build());
            }
            else*/
            {
                result = await this.BackupPolicyProvider.GetPolicyAsync(policyName);
            }

            return result;
        }
    }
}
