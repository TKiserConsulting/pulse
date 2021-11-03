namespace Webfarm.Sdk.Persistence.Migrations
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public abstract class DatabaseTypeAttribute : Attribute
    {
        protected DatabaseTypeAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
