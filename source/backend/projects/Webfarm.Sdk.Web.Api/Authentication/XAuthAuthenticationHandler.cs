namespace Webfarm.Sdk.Web.Api.Authentication
{
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Webfarm.Sdk.Web.Api.Data;

    public class XAuthAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AuthenticationSettings authSettings;

        public XAuthAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            [NotNull] IOptions<AuthenticationSettings> authSettings)
            : base(options, logger, encoder, clock)
        {
            this.authSettings = authSettings.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Task<AuthenticateResult> result;

            if (!this.Request.Headers.ContainsKey("X-Auth-Token"))
            {
                result = Task.FromResult(AuthenticateResult.NoResult());
            }
            else
            {
                string serviceKey = null;

                try
                {
                    var headerToken = this.Request.Headers["X-Auth-Token"];

                    if (this.authSettings?.Tokens.Any() ?? false)
                    {
                        foreach (var token in this.authSettings.Tokens)
                        {
                            if (headerToken == token.Token)
                            {
                                serviceKey = token.ServiceKey;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(serviceKey))
                    {
                        result = Task.FromResult(AuthenticateResult.Fail("Invalid token"));
                    }
                    else
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, serviceKey) };
                        var identity = new ClaimsIdentity(claims, this.Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
                        result = Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                }
                #pragma warning disable CA1031 // Do not catch general exception types
                catch
                #pragma warning restore CA1031 // Do not catch general exception types
                {
                    result = Task.FromResult(AuthenticateResult.Fail("Invalid X-Auth-Token Header"));
                }
            }

            return result;
        }
    }
}
