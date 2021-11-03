namespace Webfarm.Sdk.Web.Api.Middlewares
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Webfarm.Sdk.Common.Formatting;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class EnvironmentInfoMiddleware
    {
        private readonly EnvironmentInfoTextOutputFormatter formatter;

        #pragma warning disable CA1801

        // ReSharper disable once UnusedParameter.Local
        public EnvironmentInfoMiddleware(RequestDelegate next)
        {
            var applicationName = Environment.GetEnvironmentVariable("ApplicationName");
            this.formatter = new EnvironmentInfoTextOutputFormatter(applicationName);
        }

        #pragma warning restore CA1801

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke([NotNull] HttpContext context, CancellationToken cancellationToken = default)
        {
            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            await this.formatter.WriteAsync(context.Response.Body, cancellationToken);
        }
    }
}
