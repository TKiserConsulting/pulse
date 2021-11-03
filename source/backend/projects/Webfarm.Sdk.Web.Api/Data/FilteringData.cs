namespace Webfarm.Sdk.Web.Api.Data
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(BinderType = typeof(FilterModelBinder))]
    public class FilteringData : Dictionary<string, string>
    {
    }
}
