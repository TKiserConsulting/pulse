// ReSharper disable UnusedMember.Global

using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]

namespace Pulse.Api.Configuration
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Business;
    using Data.Auth;
    using Hubs;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.FeatureManagement;
    using Microsoft.Net.Http.Headers;
    using NJsonSchema;
    using NSwag;
    using NSwag.Generation.Processors.Security;
    using Persistence;
    using Serilog;
    using Webfarm.Sdk.Common.Authorization;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Persistence;
    using Webfarm.Sdk.Persistence.Configuration;
    using Webfarm.Sdk.Web.Api.Configuration;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class Startup
    {
        private static readonly ILogger Logger = Log.ForContext<Startup>();

        #region Constructors

        public Startup(IConfiguration configuration)
        {
            Logger.Information(
                "Starting {ApplicationName} version {Version}",
                Environment.GetEnvironmentVariable("ApplicationName"),
                this.GetType().Assembly.EffectiveVersion());

            this.Configuration = configuration;
        }

        #endregion

        #region Properties

        private IConfiguration Configuration { get; }

        #endregion

        #region Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureManagement();

            services
                .Configure<ApplicationDatabaseOptions>(this.Configuration)
                .PostConfigure<ApplicationDatabaseOptions>(opt => opt.DatabaseSchemaName = PulseDbContext.Schema);

            services
                .Configure<AuthorizationOptions>(opt =>
                {
                    opt.AuthorizationStrategy =
                        this.Configuration.GetValue<bool>("FeatureManagement:Authorization")
                            ? AppGrantManager.Key
                            : AllowAllGrantManager.Key;
                });

            services.AddHttpBindings();
            services.AddApiDefaults(
                this.Configuration, omitAuthorizeFilter: true, transactional: true);

            services.AddAuthenticationDefaults(
                this.Configuration,
                optionsCallback: this.HandleJwtFromQueryString);
            services.AddAuthorizationDefaults(options =>
            {
                options.AddPolicy("SignalRService", policy =>
                {
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            services.AddHealthChecks()
                .AddCheck<ConnectionHealthCheck>(ConnectionHealthCheck.Key);
            services.AddSingleton<ConnectionHealthCheck>();

            services.AddResponseCompression();

            this.AddSwagger(services);

            services.AddSpaStaticFiles(options => options.RootPath = "wwwroot");

            services.AddDbContext<PulseDbContext>(builder =>
            {
                var options = this.Configuration.Get<ApplicationDatabaseOptions>();

                #if DEBUG
                builder.EnableSensitiveDataLogging();
                #endif

                builder
                    .UseNpgsql(
                        options.DefaultConnectionString,
                        opt => opt.SetPostgresVersion(new Version(10, 11)));
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 1;
                })
                .AddEntityFrameworkStores<PulseDbContext>()
                .AddDefaultTokenProviders()
                ;

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            //AuditExtensions.AddAudit();
        }

        public void Configure(
            IApplicationBuilder app,
            [NotNull] IFeatureManager featureManager)
        {
            app.UseForwardedHeaders();

            app.UseSerilogRequestLogging(opt =>
            {
                opt.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // remote_addr is a address of reverse proxy and we do not care about it
                    // diagnosticContext.Set("remote_addr", httpContext.Connection.RemoteIpAddress);
                    diagnosticContext.Set("http_referrer", httpContext.Request.Headers[HeaderNames.Referer].FirstOrDefault());
                    diagnosticContext.Set("http_user_agent", httpContext.Request.Headers[HeaderNames.UserAgent].FirstOrDefault());
                    diagnosticContext.Set("http_x_forwarded_for", httpContext.Request.Headers[ForwardedHeadersDefaults.XForwardedForHeaderName].FirstOrDefault());
                };
            });

            app.UseResponseCompression();

            app.UseCommonTracing();
            app.UseCommonExceptionHandler();

            app.UseSwaggerDefaults(featureManager, settings =>
            {
                settings.PostProcess = (document, _) =>
                {
                    document.Schemes.Clear();
                    document.Host = null;
                    document.BasePath = "/";
                };
            });

            app.UseCommonHealthChecks<Startup>();

            app.UseCommonAuthentication();
            app.UseCommonRouting(
                requireAuthorization: true,
                environmentEndpointPath: "env",
                endpointsCallback: (endpoints) =>
                {
                    endpoints.MapHub<InstructorHub>("/hubs/instructor");
                    endpoints.MapHub<StudentHub>("/hubs/student");
                });

            app.UseSpaStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(366)
                    };
                }
            });

#if !DEBUG
            app.UseSpa(spa => {});
#endif

            app.UseOperationErrorBasedStatusCodePages();

            //app.UseAudit();
        }

        private void HandleJwtFromQueryString(JwtBearerOptions options)
        {
            // We have to hook the OnMessageReceived event in order to
            // allow the JWT authentication handler to read the access
            // token from the query string when a WebSocket or 
            // Server-Sent Events request comes in.

            // Sending the access token in the query string is required due to
            // a limitation in Browser APIs. We restrict it to only calls to the
            // SignalR hub in this code.
            // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
            // for more information about security considerations when using
            // the query string to transmit the access token.
            options.Events.OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for hubs...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs"))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            };
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddOpenApiDocument(
                config =>
                {
                    config.SchemaType = SchemaType.OpenApi3;
                    config.OperationProcessors.Insert(0, new RangeModelExclusionProcessor());
                    config.OperationProcessors.Add(new XOperationNameOperationProcessor());

                    var schemeBearer = new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the text box: Bearer {your JWT token}."
                    };

                    config.DocumentProcessors.Add(new SecurityDefinitionAppender("Bearer", Enumerable.Empty<string>(), schemeBearer));

                    config.PostProcess =
                        document =>
                        {
                            document.Info.Title = "Pulse Api";
                        };
                });
        }

#endregion
    }
}
