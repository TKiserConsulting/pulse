namespace Webfarm.Sdk.Common.Authorization
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DryIocAttributes;
    using JetBrains.Annotations;

    [SingletonReuse]
    [ExportMany(ContractKey = Key, IfAlreadyExported = IfAlreadyExported.Keep)]
    public class AllowAllGrantManager : IGrantManager
    {
        public const string Key = "allow-all";

        [NotNull]
        public Task<GrantResult> ValidateGrants([NotNull] GrantInput input, CancellationToken cancellationToken = default)
        {
            var result = new GrantResult
            {
                Allow = true,
                GrantsAvailability = input.Grants.Select(
                    g => new GrantAvailabilityData
                    {
                        Granted = true, Grant = g
                    }).ToArray()
            };

            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(result);
        }
    }
}
