namespace Webfarm.Sdk.Web.Api.Filters
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    #pragma warning disable SA1649 // File name should match first type name
    public sealed class CustomApiTransactionAttribute : ActionFilterAttribute
    #pragma warning restore SA1649 // File name should match first type name
    {
    }
}
