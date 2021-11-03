namespace Webfarm.Sdk.Web.Api.Filters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Formatters;

    public sealed class ResponseCodeAttribute : ResultFilterAttribute, IApiResponseMetadataProvider
    {
        public ResponseCodeAttribute(HttpStatusCode statusCode) => this.StatusCode = (int)statusCode;

        public int StatusCode { get; }

        public Type Type { get; } = typeof(void);

        public override void OnResultExecuted([NotNull] ResultExecutedContext context)
        {
            Contract.Assert(context != null);
            context.HttpContext.Response.StatusCode = this.StatusCode;
        }

        void IApiResponseMetadataProvider.SetContentTypes(MediaTypeCollection contentTypes)
        {
        }
    }
}
