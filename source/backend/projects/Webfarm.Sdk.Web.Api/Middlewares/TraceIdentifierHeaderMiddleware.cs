namespace Webfarm.Sdk.Web.Api.Middlewares
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public class TraceIdentifierHeaderMiddleware
    {
        private readonly RequestDelegate next;
        private readonly TraceIdentifierMiddlewareOptions options;

        public TraceIdentifierHeaderMiddleware([NotNull] RequestDelegate next, [NotNull] IOptions<TraceIdentifierMiddlewareOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.next = next ?? throw new ArgumentNullException(nameof(next));

            this.options = options.Value;
        }

        public Task Invoke([NotNull] HttpContext context)
        {
            Contract.Assert(context != null);
            Debug.Assert(context.Request != null, "context.Request != null");
            if (context.Request.Headers.TryGetValue(this.options.Header, out var traceIdentifier))
            {
                context.TraceIdentifier = traceIdentifier;
            }

            if (this.options.IncludeInResponse)
            {
                // apply the correlation ID to the response header for client side tracking
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(this.options.Header, new[] { context.TraceIdentifier });
                    return Task.CompletedTask;
                });
            }

            return this.next(context);
        }
    }
}
