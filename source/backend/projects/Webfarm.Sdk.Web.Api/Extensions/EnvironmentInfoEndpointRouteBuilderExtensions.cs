namespace Webfarm.Sdk.Web.Api.Extensions
{
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Webfarm.Sdk.Web.Api.Middlewares;

    public static class EnvironmentInfoEndpointRouteBuilderExtensions
    {
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IEndpointConventionBuilder MapEnvironmentInfo(
            [NotNull] this IEndpointRouteBuilder endpoints, string pattern)
        {
            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware<EnvironmentInfoMiddleware>()
                .Build();

            return endpoints.Map(pattern, pipeline).WithDisplayName("Environment info");
        }
    }
}
