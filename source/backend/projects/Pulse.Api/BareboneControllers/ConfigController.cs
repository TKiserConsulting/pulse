namespace Pulse.Api.BareboneControllers
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NSwag.Annotations;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data.ComponentModel;

    [GrantAuthorizationSkip]
    [Route("[controller]")]
    public class ConfigController : Controller
    {
        [ResponseCache(Duration = 5)]
        [AllowAnonymous]
        [NotNull]
        [HttpGet]
        [Route("")]
        [SwaggerResponse(typeof(object))]
        public object RetrieveConfig()
        {
            return new
            {
                Version = this.GetType().EffectiveVersion(),
                DefaultUiLanguage = "en",
                UiLanguages = new[] { "en" },
                Timestamp = DateTimeOffset.Now
            };
        }
    }
}
