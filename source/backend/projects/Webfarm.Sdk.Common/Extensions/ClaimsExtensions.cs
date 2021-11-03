namespace Webfarm.Sdk.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using JetBrains.Annotations;

    public static class ClaimsExtensions
    {
        [NotNull]
        public static string AuditId(this IExecutionContext context)
        {
            var id = "unknown";
            if (context?.Principal != null)
            {
                id = $"{context.UserId()}:{context.Username(false)}:{context.Email()}".Trim(':');
            }

            return id;
        }

        [NotNull]
        public static string UserId([NotNull] this IExecutionContext context)
        {
            Contract.Assert(context.Principal != null);
            var id = context.Principal.UserId();
            return id;
        }

        [NotNull]
        public static string Username([NotNull] this IExecutionContext context, bool useIdWhenEmpty = true)
        {
            Contract.Assert(context.Principal != null);
            var username = context.Principal.Username(useIdWhenEmpty);
            return username;
        }

        [NotNull]
        public static string Email([NotNull] this IExecutionContext context)
        {
            Contract.Assert(context.Principal != null);
            var username = context.Principal.Email();
            return username;
        }

        [NotNull]
        public static string UserId([NotNull] this IPrincipal principal)
        {
            var id = principal.Claims()
                .Where(cl => cl.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .First();
            return id;
        }

        public static T Role<T>([NotNull] this IPrincipal principal)
            where T : struct
        {
            var role = principal.Claims()
                .Where(cl => cl.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .First();
            return Enum.Parse<T>(role);
        }

        [NotNull]
        public static string Username([NotNull] this IPrincipal principal, bool useIdWhenEmpty = true)
        {
            var name = principal.Claims()
                .Where(cl => cl.Type == ClaimTypes.Name)
                .Select(c => c.Value)
                .FirstOrDefault();
            name ??= (useIdWhenEmpty ? principal.Identity?.Name : null) ?? string.Empty;

            return name;
        }

        [NotNull]
        public static string Email([NotNull] this IPrincipal principal)
        {
            var name = principal.Claims()
                .Where(cl => cl.Type == ClaimTypes.Email)
                .Select(c => c.Value)
                .FirstOrDefault() ?? string.Empty;

            return name;
        }

        [NotNull]
        public static IEnumerable<Claim> Claims([NotNull] this IPrincipal principal)
        {
            IEnumerable<Claim> claims;
            if (principal is ClaimsPrincipal cp)
            {
                claims = cp.Claims;
            }
            else if (principal.Identity is ClaimsIdentity ci)
            {
                claims = ci.Claims;
            }
            else
            {
                claims = Array.Empty<Claim>();
            }

            return claims;
        }
    }
}
