namespace Webfarm.Sdk.Data
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using Webfarm.Sdk.Common;

    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext()
        {
            this.Data = new Dictionary<string, object>();
        }

        public IPrincipal Principal { get; set; }

        public IDictionary<string, object> Data { get; }
    }
}
