namespace Webfarm.Sdk.Web.Api.Authentication
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using DryIoc;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Caching.Memory;
    using Serilog;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class JwtAuthenticationHandler : DelegatingHandler
    {
        private const string JwtTokenCacheKey = nameof(JwtAuthenticationHandler) + "_jwttoken";
        private const int CacheExpirationDiffMin = 1;

        private readonly IResolver resolver;
        private readonly IMemoryCache memoryCache;

        private JwtIntegrationOptions settings;
        private string serviceKey;
        private string issuer;
        private string audience;
        private TimeSpan expiresSpan;

        public JwtAuthenticationHandler(IResolver resolver, IMemoryCache memoryCache)
        {
            this.resolver = resolver;
            this.memoryCache = memoryCache;
        }

        public Task Initialize(string key, JwtIntegrationOptions jwtSettings)
        {
            this.serviceKey = key;
            this.settings = jwtSettings;

            this.issuer = this.settings.Issuer;
            this.audience = this.settings.Audience;
            var expiresString = this.settings.Expires;
            this.expiresSpan = expiresString != null
                ? TimeSpan.Parse(expiresString, DateTimeFormatInfo.InvariantInfo)
                : TimeSpan.FromMinutes(5);
            if (this.expiresSpan.TotalMinutes < CacheExpirationDiffMin)
            {
                throw new ArgumentException($"Expires can not be less than {CacheExpirationDiffMin} minutes");
            }

            return Task.CompletedTask;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            [NotNull] HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (this.settings != null)
            {
                var cacheKey = $"{JwtTokenCacheKey}_{this.serviceKey}";
                var token = await this.memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    // expire cache earlier than token
                    var expires = DateTime.UtcNow.Add(this.expiresSpan);
                    entry.AbsoluteExpiration = expires.Subtract(TimeSpan.FromMinutes(CacheExpirationDiffMin));

                    var jwt = await this.GenerateJwtToken(expires);

                    Log.Logger.Verbose($"Generated jwt token. Key: {this.serviceKey}, Expires: {expires}, Token: {jwt}.");
                    return jwt;
                });

                request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            }
            else
            {
                Log.Warning($"JwtAuthenticationHandler misses Settings");
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private Task<string> GenerateJwtToken(DateTime expires)
        {
            var certificate = this.resolver.Resolve<X509Certificate2>("auth");

            // [as] do we need other claims here
            var claims = new[] { new Claim(ClaimTypes.Name, this.audience) };
            var token = certificate.GenerateJwtToken(claims, expires, this.audience, this.issuer);
            return Task.FromResult(token);
        }
    }
}
