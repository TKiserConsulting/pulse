using System;
using Microsoft.AspNetCore.Authorization;

namespace Webfarm.Sdk.Web.Api.Authorization
{
    [Obsolete("not used")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PermissionAttribute : AuthorizeAttribute
    {
        public const string PolicyName = "Permission";

        public PermissionAttribute(string name)
            : base(PolicyName)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
