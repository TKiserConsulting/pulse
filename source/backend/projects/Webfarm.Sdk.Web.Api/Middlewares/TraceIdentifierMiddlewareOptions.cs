namespace Webfarm.Sdk.Web.Api.Middlewares
{
    public class TraceIdentifierMiddlewareOptions
    {
        private const string DefaultHeader = "X-Trace-Identifier";

        public string Header { get; set; } = DefaultHeader;

        public bool IncludeInResponse { get; set; } = true;
    }
}
