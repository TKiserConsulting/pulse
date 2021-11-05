namespace Pulse.Api.BareboneControllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Extensions;
    using Data;
    using Data.Auth;
    using MassTransit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Models.Authentication;
    using Models.Users;
    using Persistence;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data.Exceptions;

    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private const int AccessTokenExpiresMin = 15;

        private readonly IConfiguration configuration;
        private readonly PulseDbContext db;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthenticationController(
            IConfiguration configuration,
            PulseDbContext db,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.configuration = configuration;
            this.db = db;
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("signin")]
        public async Task<SigninResultDto> Signin(SigninRequestDto model, CancellationToken cancellationToken)
        {
            model ??= new SigninRequestDto();

            var user = await this.userManager.FindByNameAsync(model.Email);
            user.DemandUser();

            var passwordResult = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordResult.Succeeded)
            {
                throw new UnauthenticatedException("User cannot be authenticated.");
            }

            var accessToken = await this.GenerateAccessToken(user, cancellationToken);
            var refreshToken = await this.CreateRefreshToken(user, cancellationToken);

            var signinResult = new SigninResultDto
            {
                TokenInfo = new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token
                },
                User = this.mapper.Map<UserDetailsDto>(user)
            };

            return signinResult;
        }

        [HttpPost("refresh")]
        public async Task<RefreshResultDto> Refresh(RefreshRequestDto model, CancellationToken cancellationToken)
        {
            var (principal, _) = this.ParseAccessToken(model.TokenInfo.AccessToken);

            var refreshToken = await this.db.IdentityRefreshTokens
                .SingleOrDefaultAsync(t => t.Token == model.TokenInfo.RefreshToken, cancellationToken);
            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("User cannot be authenticated.");
            }

            if (refreshToken.ApplicationUserId != Guid.Parse(principal.UserId()))
            {
                throw new UnauthorizedAccessException("User cannot be authenticated.");
            }

            if (refreshToken.Expired < DateTimeOffset.Now)
            {
                throw new UnauthorizedAccessException("User cannot be authenticated.");
            }

            var user = await this.userManager.FindByNameAsync(principal.Username());
            if (user == null)
            {
                throw new UnauthorizedAccessException("User cannot be authenticated.");
            }

            var accessToken = await this.GenerateAccessToken(user, cancellationToken);
            var newRefreshToken = await this.CreateRefreshToken(user, cancellationToken);

            var signinResult = new RefreshResultDto
            {
                TokenInfo = new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken.Token
                }
            };

            return signinResult;
        }

        [HttpPost]
        [Route("signout")]
        public async Task Signout(CancellationToken cancellationToken)
        {
            var principal = this.User;

            if (principal != null && (principal.Identity?.IsAuthenticated ?? false))
            {
                // remove all refresh tokens
                var identityUserId = Guid.Parse(principal.UserId());
                await this.RemoveRefreshTokens(identityUserId, cancellationToken);

                // signout
                await this.signInManager.SignOutAsync();
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<RegisterResultDto> RegisterAccount(RegisterRequestDto model, CancellationToken cancellationToken)
        {
            var instance = this.mapper.Map<ApplicationUser>(model);
            instance.UserName = instance.Email;
            instance.Id = NewId.NextGuid();

            // create user
            await this.userManager.CreateAsync(instance, model.Password);

            // attach to role
            await this.userManager.AddToRoleAsync(instance, UserRole.Instructor.ToString());

            // copy emoticons
            var emoticons = await this.db.Emoticons.AsNoTracking().ToListAsync(cancellationToken);
            var instructorEmoticons = this.mapper.Map<InstructorEmoticon[]>(emoticons);
            foreach (var emoticon in instructorEmoticons)
            {
                emoticon.InstructorId = instance.Id;
            }

            await this.db.InstructorEmoticons.AddRangeAsync(instructorEmoticons, cancellationToken);

            await this.db.SaveChangesAsync(cancellationToken);

            var accessToken = await this.GenerateAccessToken(instance, cancellationToken);
            var refreshToken = await this.CreateRefreshToken(instance, cancellationToken);

            return new RegisterResultDto
            {
                User = this.mapper.Map<UserDetailsDto>(instance),
                TokenInfo = new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token
                }
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<IdentityRefreshToken> CreateRefreshToken(ApplicationUser identityUser, CancellationToken cancellationToken)
        {
            // remove if any
            await this.RemoveRefreshTokens(identityUser.Id, cancellationToken);

            // create new
            var refreshToken = new IdentityRefreshToken
            {
                Id = NewId.NextGuid(),
                Token = GenerateRefreshToken(),
                ApplicationUserId = identityUser.Id,
                CreatedBy = identityUser.UserName,
                Expired = DateTimeOffset.Now.AddDays(30)
            };

            await this.db.IdentityRefreshTokens.AddAsync(refreshToken, cancellationToken);

            await this.db.SaveChangesAsync(cancellationToken);

            return refreshToken;
        }

        private async Task RemoveRefreshTokens(Guid applicationUserId, CancellationToken cancellationToken)
        {
            var refreshTokens = await this.db.IdentityRefreshTokens
                .Where(t => t.ApplicationUserId == applicationUserId)
                .ToListAsync(cancellationToken);

            this.db.IdentityRefreshTokens.RemoveRange(refreshTokens);

            await this.db.SaveChangesAsync(cancellationToken);
        }

        private async Task<string> GenerateAccessToken(ApplicationUser user, CancellationToken cancellationToken)
        {
            var expiresMinutes = this.configuration.GetValue<int?>("Tokens:Expires");
            var encryptionKey = this.configuration.GetValue<string>("Tokens:Key");
            var securityKey = Encoding.UTF8.GetBytes(encryptionKey);
            var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

            var roleInstance = await this.db.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(
                    this.db.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role
                   ).
                SingleAsync(cancellationToken);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleInstance.Name)
            };

            var isSystemAdmin = await this.db.UserClaims.AnyAsync(
                c => c.ClaimType == MetadataInfo.AppClaims.ClaimType && c.ClaimValue == MetadataInfo.AppClaims.Administrator,
                cancellationToken);
            if (isSystemAdmin)
            {
                claims.Add(new Claim(MetadataInfo.AppClaims.ClaimType, MetadataInfo.AppClaims.Administrator));
            }

            var token = new JwtSecurityToken(
                issuer: this.configuration.GetValue<string>("Tokens:Issuer"),
                audience: this.configuration.GetValue<string>("Tokens:Audience"),
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes ?? AccessTokenExpiresMin),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        // ReSharper disable once UnusedTupleComponentInReturnValue
        private (ClaimsPrincipal principal, JwtSecurityToken token) ParseAccessToken(string accessToken)
        {
            var encryptionKey = this.configuration.GetValue<string>("Tokens:Key");
            var securityKey = Encoding.UTF8.GetBytes(encryptionKey);
            var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (principal == null || !(securityToken is JwtSecurityToken jwtSecurityToken))
            {
                throw new UnauthenticatedException("User cannot be authenticated.");
            }

            return (principal, jwtSecurityToken);
        }
    }
}
