namespace Webfarm.Sdk.Persistence.Migrations
{
    using DryIoc;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator container)
        {
            container.RegisterLogging();
        }
    }
}
