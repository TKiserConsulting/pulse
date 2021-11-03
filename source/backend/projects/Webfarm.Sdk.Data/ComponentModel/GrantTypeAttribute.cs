namespace Webfarm.Sdk.Data.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GrantTypeAttribute : Attribute, IGrantType
    {
        public GrantTypeAttribute(string grantType)
        {
            this.GrantType = grantType;
        }

        public string GrantType { get; }
    }
}
