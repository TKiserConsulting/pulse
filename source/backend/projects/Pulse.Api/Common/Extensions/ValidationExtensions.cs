namespace Pulse.Api.Common.Extensions
{
    using System;
    using System.Security.Principal;
    using Data;
    using Data.Auth;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Identity;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data.Exceptions;

    public static class ValidationExtensions
    {
#pragma warning disable CA1054 // Uri parameters should not be strings
        public static void DemandValid([NotNull] this SignInResult result, string key)
        {
            if (!result.Succeeded)
            {
                throw new ApplicationValidationException(
                    new ApplicationValidationResult(key, errorCode: "WrongPassword"));
            }
        }

        public static void DemandValid([NotNull] this IdentityResult result, string key)
        {
            if (!result.Succeeded)
            {
                throw new ApplicationValidationException(
                    new ApplicationValidationResult(key, errorCode: "WrongPassword"));
            }
        }

        public static void DemandUser(this ApplicationUser user)
        {
            if (user == null)
            {
                throw new UnauthenticatedException("User cannot be authenticated.");
            }
        }

        public static void DemandUserCreate(this IPrincipal principal, UserRole roleToCreate)
        {
            var currentRole = principal.Role<UserRole>();
            var allow = currentRole == UserRole.Admin ||
                        (currentRole == UserRole.Instructor &&
                         (roleToCreate == UserRole.Instructor));
            if (!allow)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
