namespace Infoplus.Askod.Sdk.Web.Api.Extensions
{
    using Microsoft.AspNetCore.Builder;

    public static class BootstrapExtensions
    {
        public static void UseExceptionHandlingApi(this IApplicationBuilder app)
        {
            /*
            app.UseMiddleware(typeof(CorrelationIdMiddleware));
            app.UseMiddleware(typeof(TrackingApiMiddleware));
            app.UseMiddleware(typeof(ErrorHandlingApiMiddleware));
            */
        }
    }
}
