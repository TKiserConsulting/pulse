namespace Webfarm.Sdk.Data.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class GrantAuthorizationSkipAttribute : Attribute
    {
    }
}
