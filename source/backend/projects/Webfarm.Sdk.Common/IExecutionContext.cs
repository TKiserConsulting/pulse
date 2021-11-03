namespace Webfarm.Sdk.Common
{
    using System.Collections.Generic;
    using System.Security.Principal;

    public interface IExecutionContext
    {
        IPrincipal Principal { get; }

        IDictionary<string, object> Data { get; }
    }
}
