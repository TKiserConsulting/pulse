namespace Webfarm.Sdk.Common.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGrantManager
    {
        Task<GrantResult> ValidateGrants(GrantInput input, CancellationToken cancellationToken = default);
    }
}
