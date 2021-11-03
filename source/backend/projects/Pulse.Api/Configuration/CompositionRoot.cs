namespace Pulse.Api.Configuration
{
    using AutoMapper;
    using Business;
    using Common.Managers;
    using DryIoc;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence.Configuration;
    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class CompositionRoot
    {
        public CompositionRoot(IContainer container)
        {
            container.RegisterLogging();
            container.Register<IExecutionContext, ExecutionContextProxy>(Reuse.Singleton);
            container.Register<AuthManager, AuthManager>(Reuse.Singleton);

            container.Resolve<PersistenceCompositionModule>();
            container.Resolve<BusinessCompositionModule>();

            container.RegisterDelegate(CreateMapperConfiguration, Reuse.Singleton);
            container.RegisterDelegate(CreateMapper, Reuse.InCurrentScope);
        }

        private static IConfigurationProvider CreateMapperConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Startup).Assembly);
            });
            return config;
        }

        private static IMapper CreateMapper(IResolverContext resolver)
        {
            var config = resolver.GetRequiredService<IConfigurationProvider>();
            var mapper = new Mapper(config, resolver.GetRequiredService);
            return mapper;
        }
    }
}
