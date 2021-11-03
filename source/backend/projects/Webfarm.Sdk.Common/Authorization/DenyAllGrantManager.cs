namespace Webfarm.Sdk.Common.Authorization
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DryIocAttributes;
    using JetBrains.Annotations;

    [SingletonReuse]
    [ExportMany(ContractKey = "deny-all", IfAlreadyExported = IfAlreadyExported.Keep)]
    public class DenyAllGrantManager : IGrantManager
    {
        [NotNull]
        public Task<GrantResult> ValidateGrants([NotNull] GrantInput input, CancellationToken cancellationToken = default)
        {
            var result = new GrantResult
            {
                Allow = false,
                GrantsAvailability = input.Grants.Select(
                    g => new GrantAvailabilityData
                    {
                        Granted = false, Grant = g
                    }).ToArray()
            };

            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(result);
        }
    }
}
