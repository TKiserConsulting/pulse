namespace Webfarm.Sdk.Web.Api.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ActionConstraints;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Webfarm.Sdk.Data.ComponentModel;

    internal class GrantContext
    {
        private static readonly Dictionary<string, string> ActionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { HttpMethods.Post, "create" },
            { HttpMethods.Get, "read" },
            { HttpMethods.Put, "update" },
            { HttpMethods.Patch, "update" },
            { HttpMethods.Delete, "delete" },
        };

        private string grantType;
        private string grantAction;

        public GrantContext([NotNull] Endpoint resource)
        {
            this.Initialize(resource);
        }

        public string Controller { get; set; }

        public string Method { get; set; }

        public string Prefix { get; set; }

        // ReSharper disable once AnnotateNotNullTypeMember
        public string GrantType
        {
            get
            {
                var result = this.grantType
                             ?? this.BuildGrantType();
                return (result ?? "default").ToLowerInvariant();
            }
            set => this.grantType = value;
        }

        // ReSharper disable once AnnotateNotNullTypeMember
        public string GrantAction
        {
            get
            {
                var result = this.grantAction;
                if (result == null)
                {
                    if (this.Method != null && ActionMap.ContainsKey(this.Method))
                    {
                        result = ActionMap[this.Method];
                    }
                }

                return (result ?? "default").ToLowerInvariant();
            }
            set => this.grantAction = value;
        }

        public bool SkipAuthorization { get; set; }

        [NotNull]
        private static IEnumerable<TAttribute> GetAttributes<TAttribute>([CanBeNull] ICustomAttributeProvider memberInfo)
        {
            IEnumerable<TAttribute> result;
            if (memberInfo == null)
            {
                result = Array.Empty<TAttribute>();
            }
            else
            {
                result = memberInfo.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>();
            }

            return result;
        }

        private void Initialize([NotNull] Endpoint resource)
        {
            var action = resource.Metadata.GetMetadata<ControllerActionDescriptor>();

            var grantTypePrefixAttribute = GetAttributes<GrantTypePrefixAttribute>(
                action?.ControllerTypeInfo).FirstOrDefault();
            var grantTypeAttribute = GetAttributes<IGrantType>(
                action?.ControllerTypeInfo).FirstOrDefault();
            var grantActionAttribute = GetAttributes<IGrantAction>(
                action?.MethodInfo).FirstOrDefault();

            this.SkipAuthorization =
                GetAttributes<GrantAuthorizationSkipAttribute>(
                    action?.MethodInfo).Any() ||
                (GetAttributes<GrantAuthorizationSkipAttribute>(
                    action?.ControllerTypeInfo).Any() && grantActionAttribute == null);

            this.Controller = action?.ControllerName ?? "Default";
            this.Prefix = grantTypePrefixAttribute?.Prefix;
            this.Method = action?.ActionConstraints.OfType<HttpMethodActionConstraint>().SelectMany(c => c.HttpMethods)
                .FirstOrDefault() ?? HttpMethods.Get;
            this.GrantAction = grantActionAttribute?.GrantAction;
            this.GrantType = grantActionAttribute?.GrantType ?? grantTypeAttribute?.GrantType;
        }

        [CanBeNull]
        private string BuildGrantType()
        {
            var result = this.Controller ?? string.Empty;
            result = string.Concat(result.Select(
                (x, i) => i > 0 && char.IsUpper(x) ? "-" + x : x.ToString()));

            if (!string.IsNullOrEmpty(this.Prefix))
            {
                result = $"{this.Prefix}.{result}";
            }

            return result.ToLowerInvariant();
        }
    }
}
