namespace Webfarm.Sdk.Daemon.Configuration
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Webfarm.Sdk.Web.Api.Extensions;

    #pragma warning disable SA1649 // File name should match first type name
    public class WebHostStartup
    #pragma warning restore SA1649 // File name should match first type name
    {
        protected const string DefaultResourceCulture = "uk";

        #region Constructors

        public WebHostStartup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        #endregion

        #region Properties

        protected virtual bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;

        protected virtual bool OmitAuthorizeFilter { get; set; }

        protected virtual bool UseCentralRoutePrefix { get; set; } = true;

        protected virtual bool Transactional { get; set; }

        protected IConfiguration Configuration { get; }

        #endregion

        #region Methods

        // ReSharper disable once UnusedMember.Global
        public virtual void ConfigureServices(IServiceCollection services)
        {
            /* services.AddLocalization(options => options.ResourcesPath = "resources"); */
            services.AddHttpBindings();

            services.AddApiDefaults(
                this.Configuration,
                mvcBuilder: this.BuildMvc,
                assumeDefaultVersionWhenUnspecified: this.AssumeDefaultVersionWhenUnspecified,
                omitAuthorizeFilter: this.OmitAuthorizeFilter,
                useCentralRoutePrefix: this.UseCentralRoutePrefix,
                transactional: this.Transactional);

            services.AddHealthChecks();
        }

        public virtual void Configure([NotNull] IApplicationBuilder app)
        {
            app
                .UseCommonTracing()
                .UseCommonLocalization(DefaultResourceCulture, DefaultResourceCulture)
                .UseCommonExceptionHandler();

            this.ConfigureInner(app);

            app
                .UseCommonHealthChecks()
                .UseCommonRoutingWithAuthentication();
        }

        protected virtual void ConfigureInner([NotNull] IApplicationBuilder app)
        {
        }

        protected virtual void BuildMvc(IMvcBuilder builder)
        {
            builder.AddApplicationPart(this.GetType().Assembly);
        }

        #endregion
    }
}
