namespace Infoplus.Askod.Portal.Web.Api.Common.Authorization
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class GrantAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefix = "grant";

        private string grantType;
        private string grantAction;

        public GrantAuthorizeAttribute(string grantType, string grantAction)
        {
            this.grantType = grantType;
            this.grantAction = grantAction;
            this.Policy = this.BuildPolicy();
        }

        public string GrantType
        {
            get => this.grantType;
            set
            {
                this.grantType = value;
                this.Policy = this.BuildPolicy();
            }
        }

        public string GrantAction
        {
            get => this.grantAction;
            set
            {
                this.grantAction = value;
                this.Policy = this.BuildPolicy();
            }
        }

        [NotNull]
        private string BuildPolicy()
        {
            return $"{PolicyPrefix}:{this.grantType}:{this.GrantAction}";
        }
    }
}
