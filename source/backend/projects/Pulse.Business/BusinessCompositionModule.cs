namespace Pulse.Business
{
    using DryIoc;
    using DryIoc.MefAttributedModel;
    using JetBrains.Annotations;
    using Webfarm.Sdk.AutoRegistration;
    using Webfarm.Sdk.AutoRegistration.Extensions;
    using Webfarm.Sdk.Common.Authorization;

    public class BusinessCompositionModule
    {
        public BusinessCompositionModule([NotNull] IRegistrator registrator)
        {
            registrator
                .ConfigureAutoRegistration()

                .IncludeAssembly(this.GetType().Assembly)
                .Exclude(If.NotEndsWith(WellKnownAppParts.Manager))

                .Include(If.ImplementsITypeName, Then.Register().WithReuse(Reuse.Scoped))

                .ApplyAutoRegistration();

            registrator.RegisterExports(
                typeof(DefaultGrantManager),
                typeof(AllowAllGrantManager));

            registrator.Register<IGrantManager, AppGrantManager>(
                Reuse.Singleton, serviceKey: AppGrantManager.Key);
        }
    }
}
