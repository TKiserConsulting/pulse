namespace Webfarm.Sdk.Data.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AliasAttribute : Attribute
    {
        public AliasAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
