namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using JetBrains.Annotations;
    using Microsoft.IdentityModel.Tokens;

    public static class AuthenticationExtensions
    {
        public static string GenerateJwtToken(this X509Certificate2 certificate, IEnumerable<Claim> claims, DateTime? expires = null, [CanBeNull] string audience = null, [CanBeNull] string issuer = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new X509SecurityKey(certificate);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature),
                IssuedAt = DateTime.UtcNow
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
