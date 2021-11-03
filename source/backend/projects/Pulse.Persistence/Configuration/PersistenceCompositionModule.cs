namespace Pulse.Persistence.Configuration
{
    using System.Diagnostics.CodeAnalysis;
    using DryIoc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
    using Webfarm.Sdk.Persistence.Configuration;

    public class PersistenceCompositionModule : PersistenceCoreCompositionModule
    {
        public PersistenceCompositionModule([NotNull] IContainer container)
            : base(container)
        {
        }

        protected override void ConfigureExceptionPolicy(IExceptionConfigurationForExceptionType policy)
        {
            policy
                .ForExceptionType<DbUpdateException>()
                    .HandleCustom(this.PostgresExceptionHandlerType)
                        .ThenThrowNewException();
            base.ConfigureExceptionPolicy(policy);
        }
    }
}
