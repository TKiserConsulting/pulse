namespace Webfarm.Sdk.Web.Api.Data
{
    public class JwtIntegrationOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Expires { get; set; }
    }
}
