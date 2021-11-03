namespace Webfarm.Sdk.Data
{
    using System.Security.Claims;
    using System.Security.Principal;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Common;

    public class ExecutionContextPrincipal : IPrincipal
    {
        [NotNull]
        private readonly IExecutionContext executionContext;

        public ExecutionContextPrincipal([NotNull]IExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        // ReSharper disable once AnnotateCanBeNullTypeMember
        public IIdentity Identity
        {
            get
            {
                var principal = this.InnerPrincipal;
                principal ??= new ClaimsPrincipal();
                return principal.Identity;
            }
        }

        [CanBeNull]
        private IPrincipal InnerPrincipal => this.executionContext.Principal;

        public bool IsInRole(string role)
        {
            return this.InnerPrincipal?.IsInRole(role) ?? false;
        }
    }
}
