namespace Webfarm.Sdk.Data.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreOnUpdateAttribute : Attribute
    {
    }
}
