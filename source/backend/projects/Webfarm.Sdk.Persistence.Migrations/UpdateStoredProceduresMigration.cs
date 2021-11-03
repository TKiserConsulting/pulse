namespace Webfarm.Sdk.Persistence.Migrations
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentMigrator;
    using JetBrains.Annotations;

    public abstract class UpdateStoredProceduresMigration : ForwardOnlyMigration
    {
        protected UpdateStoredProceduresMigration(string namespaceValue)
        {
            this.Namespace = namespaceValue;
        }

        private string Namespace { get; }

        public override void Up()
        {
            var assembly = this.GetAssembly();
            if (assembly != null)
            {
                var manifestResourceNames = assembly.GetManifestResourceNames();
                var names = manifestResourceNames
                    .Where(n => n.Contains($".{this.Namespace}.", StringComparison.OrdinalIgnoreCase) && n.Contains(".Updatables.", StringComparison.OrdinalIgnoreCase));
                foreach (var name in names)
                {
                    this.Execute.EmbeddedScript(name);
                }
            }
        }

        [CanBeNull]
        protected virtual Assembly GetAssembly()
        {
            return this.GetType().Assembly;
        }
    }
}
