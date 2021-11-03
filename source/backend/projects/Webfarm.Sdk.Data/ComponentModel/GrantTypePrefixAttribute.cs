namespace Webfarm.Sdk.Data.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GrantTypePrefixAttribute : Attribute
    {
        public GrantTypePrefixAttribute(string prefix)
        {
            this.Prefix = prefix;
        }

        public string Prefix { get; }
    }
}
