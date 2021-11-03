namespace Infoplus.Askod.Sdk.Web.Api.Extensions
{
    using System;
    using Infoplus.Askod.Data.Metadata;
    using Microsoft.AspNetCore.Http;

    public static class HttpContextExtensions
    {
        public static Guid? GetTrackingId(this HttpContext context)
        {
            Guid? trackingId = null;
            var hasTrackingId = context?.Request?.HttpContext?.Items.ContainsKey(ExecutionContextHeaders.TrackingId);
            if (hasTrackingId.HasValue && hasTrackingId.Value)
            {
                var item = context.Request?.HttpContext?.Items[ExecutionContextHeaders.TrackingId] as string;
                trackingId = Guid.Parse(item);
            }

            return trackingId;
        }

        /*
        public static string GetUserIp(this HttpContext context)
        {
            var ip = string.Empty;
            if (context != null)
            {
                var request = context.Request;
                if (request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                else if (!string.IsNullOrEmpty(request.UserHostAddress))
                {
                    ip = request.UserHostAddress;
                }
            }

            return ip;
        }
        */
    }
}
