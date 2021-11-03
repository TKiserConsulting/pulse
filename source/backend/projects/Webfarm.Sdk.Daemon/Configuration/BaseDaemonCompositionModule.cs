namespace Webfarm.Sdk.Daemon.Configuration
{
    using System.Security.Principal;
    using DryIoc;
    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class BaseDaemonCompositionModule
    {
        protected BaseDaemonCompositionModule(IRegistrator registrator)
        {
            registrator.RegisterLogging();

            registrator.Register<IExecutionContext, ExecutionContextProxy>(Reuse.Singleton);
            registrator.Register<IPrincipal, ExecutionContextPrincipal>(Reuse.Transient);
        }
    }
}
