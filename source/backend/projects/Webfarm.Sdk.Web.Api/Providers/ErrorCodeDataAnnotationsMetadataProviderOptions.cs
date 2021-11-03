namespace Webfarm.Sdk.Web.Api.Providers
{
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class ErrorCodeDataAnnotationsMetadataProviderOptions : IConfigureOptions<MvcOptions>
    {
        public void Configure([NotNull] MvcOptions options)
        {
            Contract.Assert(options != null);

            var provider = new ErrorCodeDataAnnotationsMetadataProvider();
            options.ModelMetadataDetailsProviders.Add(provider);
        }
    }
}
