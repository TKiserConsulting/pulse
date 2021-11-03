namespace Webfarm.Sdk.Data.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class GrantActionAttribute : Attribute, IGrantAction
    {
        public GrantActionAttribute(string grantAction, string grantType = default)
        {
            this.GrantAction = grantAction;
            this.GrantType = grantType;
        }

        public string GrantType { get; set; }

        public string GrantAction { get; set; }
    }
}
