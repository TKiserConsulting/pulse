// ReSharper disable UnusedMethodReturnValue.Global
namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;

    using DryIoc;

    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;
    using Microsoft.IdentityModel.Tokens;
    using NSwag.AspNetCore;

    using StackExchange.Redis;

    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Authorization;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Exceptions;
    using Webfarm.Sdk.Web.Api.Filters;
    using Webfarm.Sdk.Web.Api.Middlewares;
    using Webfarm.Sdk.Web.Api.Providers;
    using ClaimTypes = System.Security.Claims.ClaimTypes;

#pragma warning disable CA1506
    public static class ConfigurationExtensions
#pragma warning restore CA1506
    {
        /*
        public static void UseAsGlobalLoggerConfiguration([NotNull] this IConfigurationBuilder configuration)
        {
            Contract.Assert(configuration != null);

            var config = configuration.Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Information("Logging has been configured.");
        }
        */

        public static IServiceCollection AddApplicationInvalidModelStateResponseFactory(
            this IServiceCollection services)
        {
            services.ConfigureOptions<ErrorCodeDataAnnotationsMetadataProviderOptions>();
            services.AddOptions<ApiBehaviorOptions>()
                .Configure<IOptions<ApplicationApiBehaviorOptions>>(
                    (options, o2) =>
                        options.UseApplicationInvalidModelStateResponseFactory(o2.Value.IncludeErrorDetailPolicy));

            return services;
        }

        public static void UseApplicationInvalidModelStateResponseFactory(
            [NotNull] this ApiBehaviorOptions options,
            IncludeErrorDetailPolicyType exceptionPolicy)
        {
            Contract.Assert(options != null);

            options.InvalidModelStateResponseFactory =
                context => OperationErrorResult.FromModelState(context.ModelState, context.HttpContext, exceptionPolicy);
        }

        public static IApplicationBuilder UseCommonLocalization(this IApplicationBuilder app, string defaultCulture, params string[] supportedCultures)
        {
            return app.UseRequestLocalization(BuildLocalizationOptions(defaultCulture, supportedCultures));
        }

        public static void AddApiDefaults(
            this IServiceCollection services,
            IConfiguration configuration,
            [CanBeNull] Action<MvcOptions> configure = null,
            [CanBeNull] Action<IMvcBuilder> mvcBuilder = null,
            bool assumeDefaultVersionWhenUnspecified = false,
            bool omitAuthorizeFilter = false,
            bool useCentralRoutePrefix = true,
            bool transactional = false)
        {
            var builder = services
                .AddControllers(config =>
                {
                    // This enables the AuthorizeFilter on all endpoints
                    if (!omitAuthorizeFilter)
                    {
                        // todo [ds]: AuthorizeFilter is obsolete and probably should be deleted from sdk codebase
                        // omitAuthorizeFilter tries to fix it
                        config.Filters.Add(new AuthorizeFilter());
                    }

                    if (useCentralRoutePrefix)
                    {
                        config.UseCentralRoutePrefix();
                    }

                    if (transactional)
                    {
                        config.Filters.Add<ApiTransactionAttribute>();
                    }

                    configure?.Invoke(config);
                })
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddDefaultJsonOptions();

            services
                .Configure<ApplicationApiBehaviorOptions>(configuration)
                .AddApplicationInvalidModelStateResponseFactory()
                .AddDefaultApiVersioning(assumeDefaultVersionWhenUnspecified)
                .AddRoutingLowercaseUrls();

            mvcBuilder?.Invoke(builder);
        }

        public static IMvcBuilder AddDefaultJsonOptions(
            this IMvcBuilder builder)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-3.1
            return builder
                .AddNewtonsoftJson(opt => { opt.SerializerSettings.ApplyDefaults(); });
        }

        [NotNull]
        public static IServiceCollection AddDefaultApiVersioning([NotNull] this IServiceCollection services, bool assumeDefaultVersionWhenUnspecified = true)
        {
            services.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = assumeDefaultVersionWhenUnspecified;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.ErrorResponses = new ApiVersioningResponseProvider();
                });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // ReSharper disable once StringLiteralTypo
                    options.SubstitutionFormat = "VVVV";
                    options.SubstituteApiVersionInUrl = true;
                    options.ApiVersionParameterSource = new UrlSegmentApiVersionReader();
                });

            return services;
        }

        public static IServiceCollection AddRoutingLowercaseUrls(this IServiceCollection services)
        {
            return services.AddRouting(options => { options.LowercaseUrls = true; });
        }

        public static void AddAuthenticationDefaults(
            this IServiceCollection services,
            [NotNull] IConfiguration configuration,
            [CanBeNull] Func<TokenValidatedContext, Task> fill = null,
            Action<JwtBearerOptions> optionsCallback = null)
        {
            Contract.Assert(configuration != null);

            // var authority = configuration.GetValue<string>("Tokens:Authority");
            var issuer = configuration.GetValue<string>("Tokens:Issuer");
            var audience = configuration.GetValue<string>("Tokens:Audience");

            var securityKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("Tokens:Key")));

            var validation = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.Name,
                IssuerSigningKey = securityKey,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthenticationDefaults(
                configuration,
                validation,
                fill,
                optionsCallback);
        }

        public static void AddAuthenticationDefaults(
            this IServiceCollection services,
            IConfiguration configuration,
            TokenValidationParameters validation,
            Func<TokenValidatedContext, Task> fill = null,
            Action<JwtBearerOptions> optionsCallback = null)
        {
            var apiBehaviorOptions = configuration.Get<ApplicationApiBehaviorOptions>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.RequireHttpsMetadata = false; // todo [mi] change to https
                        options.IncludeErrorDetails =
                            apiBehaviorOptions.IncludeErrorDetailPolicy == IncludeErrorDetailPolicyType.Include;
                        options.TokenValidationParameters = validation;
                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = context =>
                            {
                                context.HttpContext.RegisterPrincipal(context.Principal);

                                var res = Task.CompletedTask;
                                if (fill != null)
                                {
                                    res = fill(context);
                                }

                                return res;
                            },
                            OnChallenge = context => context.SendOperationErrorAsResult(),
                            OnForbidden = context => context.SendOperationErrorAsResult(),
                        };

                        optionsCallback?.Invoke(options);
                    });
        }

        public static void AddAuthorizationDefaults(
            this IServiceCollection services,
            Action<AuthorizationOptions> optionsCallback = null)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .AddRequirements(new ContractBasedGrantAuthorizationRequirement())
                        .Build();

                optionsCallback?.Invoke(options);
            });

            services.AddSingleton<IAuthorizationHandler, ContractBasedGrantAuthorizationHandler>();
        }

        public static void AddHttpBindings(this IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()?.HttpContext?.User);
        }

        public static IApplicationBuilder UseCommonHealthChecks<T>(this IApplicationBuilder app, string path = "/api/monitor/health")
        {
            return app.UseCommonHealthChecks(typeof(T).Assembly, path);
        }

        public static IApplicationBuilder UseCommonHealthChecks(this IApplicationBuilder app, string path = "/api/monitor/health")
        {
            return app.UseCommonHealthChecks(Assembly.GetEntryAssembly(), path);
        }

        public static IApplicationBuilder UseCommonHealthChecks(this IApplicationBuilder app, Assembly rootAssembly, string path)
        {
            var options = new HealthCheckOptions
                              {
                                  ResponseWriter = async (c, r) =>
                                      {
                                          c.Response.ContentType = "application/json";

                                          var res = new
                                                        {
                                                            rootAssembly.GetName().Name,
                                                            Timestamp = DateTime.Now,
                                                            Version = rootAssembly.EffectiveVersion(),
                                                            Status = r.Status.ToString(),
                                                            Components = r.Entries.Select(
                                                                e => new
                                                                         {
                                                                             key = e.Key,
                                                                             value = e.Value.Status.ToString()
                                                                         })
                                                        };

                                          var result = res.Serialize();

                                          await c.Response.WriteAsync(result);
                                      }
                              };
            app.UseHealthChecks(path, options);

            return app;
        }

        public static IApplicationBuilder UseCommonTracing(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware(typeof(TraceIdentifierHeaderMiddleware))
                .UseMiddleware(typeof(ExecutionContextTraceIdentifierBinderMiddleware));
        }

        public static IApplicationBuilder UseCommonAuthentication(this IApplicationBuilder app)
        {
            return app
                .UseAuthentication()
                .UseMiddleware(typeof(ExecutionContextPrincipalBinderMiddleware));
        }

        public static IApplicationBuilder UseCommonRouting(
            this IApplicationBuilder app,
            bool withAuthorization = true,
            bool requireAuthorization = false,
            Action<IEndpointRouteBuilder> endpointsCallback = null,
            string environmentEndpointPath = "/")
        {
            app.UseRouting();

            if (withAuthorization)
            {
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapEnvironmentInfo(environmentEndpointPath);
                if (requireAuthorization)
                {
                    endpoints.MapControllers().RequireAuthorization();
                }
                else
                {
                    endpoints.MapControllers();
                }

                endpointsCallback?.Invoke(endpoints);
            });

            return app;
        }

        public static IApplicationBuilder UseCommonExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                var applicationApiBehaviorOptions = errorApp.ApplicationServices.GetService<IOptions<ApplicationApiBehaviorOptions>>();

                // ReSharper disable once PossibleNullReferenceException
                var includeErrorDetailPolicy = applicationApiBehaviorOptions.Value.IncludeErrorDetailPolicy;
                var exceptionManager = new ErrorHandlingService(includeErrorDetailPolicy).ExceptionManager;

                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    var transformedException = exceptionManager.TransformException(exception, ErrorHandlingService.DefaultExceptionPolicy);

                    OperationError operationError;
                    switch (transformedException)
                    {
                        case OperationErrorApiException exc:
                            operationError = exc.Error;
                            break;
                        default:
                            operationError = new OperationError()
                                .With(HttpStatusCode.InternalServerError)
                                .With(transformedException ?? exception, includeErrorDetailPolicy);
                            break;
                    }

                    await new OperationErrorResult(operationError).ExecuteResultAsync(context);
                });
            });

            return app;
        }

        public static void UseCentralRoutePrefix([NotNull] this MvcOptions opts)
        {
            opts.UseCentralRoutePrefix(new RouteAttribute("api/{version:apiVersion}"));
        }

        public static void UseCentralRoutePrefix([NotNull] this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            Contract.Assert(opts != null);
            opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }

        public static IApplicationBuilder UseOperationErrorBasedStatusCodePages(
            this IApplicationBuilder app)
        {
            app.UseStatusCodePages(async context =>
            {
                var operationError = new OperationError();
                var statusCode = context.HttpContext.Response.StatusCode;
                switch (statusCode)
                {
                    case (int)HttpStatusCode.NotFound:
                        operationError.With(WellKnownErrorCodeTypes.ResourceNotFound);
                        break;
                    default:
                        operationError.With(WellKnownErrorCodeTypes.UnexpectedError);
                        operationError.Status = (HttpStatusCode)statusCode;
                        break;
                }

                operationError.Instance = context.HttpContext.Request.Path;

                await new OperationErrorResult(operationError).ExecuteResultAsync(context.HttpContext);
            });

            return app;
        }

        public static IApplicationBuilder UseSwaggerDefaults(this IApplicationBuilder app, [CanBeNull] IFeatureManager featureManager = null, [CanBeNull] Action<OpenApiDocumentMiddlewareSettings> configure = null)
        {
            if (featureManager == null ||
                featureManager.IsEnabledAsync(WellKnownFeatures.OpenApi).Result)
            {
                app.UseSwaggerUi3();
                app.UseOpenApi(configure);
            }

            return app;
        }

        public static IServiceCollection AddDataProtectionDefaults(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationName = configuration.GetValue<string>("ApplicationName");
            var dataProtectionBuilder = services.AddDataProtection()
                .SetApplicationName(applicationName);

            if (configuration.GetValue<bool>("FeatureManagement:DataProtectionRedis"))
            {
                var redisUri = configuration.GetValue<string>("ConnectionStrings:Redis");
                var redis = ConnectionMultiplexer.Connect(redisUri);
                dataProtectionBuilder
                    .PersistKeysToStackExchangeRedis(redis);
            }

            return services;
        }

        public static void RegisterPrincipal(this HttpContext context, IPrincipal principal)
        {
            // [as] update principal in execution context.
            // ExecutionContextPrincipalBinderMiddleware is called before actual authentication happened
            // thus given middleware can be used to set unauthorized principal.
            // can be useful to prevent null accessing execution context before auth.
            // real principal is set later
            Contract.Assert(context != null);
            var resolver = (IResolver)context.RequestServices.GetService(typeof(IResolver));
            var executionContext = resolver.Resolve<IExecutionContext>();
            ((ExecutionContextProxy)executionContext).Principal = principal;
        }

        private static RequestLocalizationOptions BuildLocalizationOptions(string defaultCulture, params string[] supportedCultures)
        {
            supportedCultures = supportedCultures.Length == 0 ? new[] { defaultCulture } : supportedCultures;
            return new RequestLocalizationOptions()
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture(defaultCulture);
        }
    }
}
