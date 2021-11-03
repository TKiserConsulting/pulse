namespace Webfarm.Sdk.Common.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;
    using DryIoc;
    using DryIocAttributes;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Options;

    [TransientReuse]
    [ExportMany(IfAlreadyExported = IfAlreadyExported.Keep)]
    public class DefaultGrantManager : IGrantManager
    {
        private readonly IResolver resolver;
        private readonly string strategy;

        public DefaultGrantManager([NotNull] IOptions<AuthorizationOptions> options, IResolver resolver)
        {
            this.resolver = resolver;
            this.strategy = options.Value.AuthorizationStrategy;
        }

        public Task<GrantResult> ValidateGrants(GrantInput input, CancellationToken cancellationToken = default)
        {
            var manager = this.resolver.Resolve<IGrantManager>(this.strategy);
            return manager.ValidateGrants(input, cancellationToken);
        }
    }
}
