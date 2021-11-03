namespace Webfarm.Sdk.Data
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Threading;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Common;

    public class ExecutionContextProxy : IExecutionContext
    {
        private static readonly AsyncLocal<IExecutionContext> LocalHolder = new AsyncLocal<IExecutionContext>();

        public IPrincipal Principal
        {
            get => Accessor().Principal;
            set => ((ExecutionContext)Accessor()).Principal = value;
        }

        public IDictionary<string, object> Data => Accessor().Data;

        [NotNull]
        private static IExecutionContext Accessor()
        {
            return LocalHolder.Value ?? (LocalHolder.Value = new ExecutionContext());
        }
    }
}
