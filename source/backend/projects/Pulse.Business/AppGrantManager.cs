// ReSharper disable UnusedMember.Local
namespace Pulse.Business
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using Data.Auth;
    using Webfarm.Sdk.Common.Authorization;
    using Webfarm.Sdk.Common.Extensions;

    public class AppGrantManager : IGrantManager
    {
        public const string Key = "app";

        public Task<GrantResult> ValidateGrants(GrantInput input, CancellationToken cancellationToken = default)
        {
            var availability = input.Grants.Select(
                g => new GrantAvailabilityData
                {
                    Grant = g,
                    Granted = this.Granted(input.Principal, g)
                }).ToArray();

            var result = new GrantResult
            {
                Allow = availability.All(a => a.Granted),
                GrantsAvailability = availability
            };

            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(result);
        }

        private bool Granted(IPrincipal principal, GrantDescriptor grant)
        {
            var role = GetRole(principal);
            var granted = this.Granted(role, grant);
            return granted;
        }

        private bool Granted(UserRole role, GrantDescriptor grant)
        {
            var granted = role switch
            {
                UserRole.Admin => CheckAdminPermission(grant),
                UserRole.Instructor => CheckInstructorPermission(grant),
                UserRole.Student => CheckStudentPermission(grant),
                _ => false,
            };

            return granted;
        }

        private static bool CheckAdminPermission(GrantDescriptor grant)
            => true;

        private static bool CheckInstructorPermission(GrantDescriptor grant)
            => IsInstructorArea(grant);

        private static bool CheckStudentPermission(GrantDescriptor grant)
            => IsStudentArea(grant);

        private static UserRole GetRole(IPrincipal principal) => principal.Role<UserRole>();

        private static bool IsAdminArea(GrantDescriptor grant) => grant.Type.StartsWith(WellKnownApplicationParts.Admin, StringComparison.OrdinalIgnoreCase);
        private static bool IsInstructorArea(GrantDescriptor grant) => grant.Type.StartsWith(WellKnownApplicationParts.Instructor, StringComparison.OrdinalIgnoreCase);
        private static bool IsStudentArea(GrantDescriptor grant) => grant.Type.StartsWith(WellKnownApplicationParts.Student, StringComparison.OrdinalIgnoreCase);
    }
}
