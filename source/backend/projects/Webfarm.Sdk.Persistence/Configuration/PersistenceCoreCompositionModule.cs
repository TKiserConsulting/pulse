namespace Webfarm.Sdk.Persistence.Configuration
{
    using System;
    using System.Linq;
    using DryIoc;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
    using Npgsql;
    using Webfarm.Sdk.Persistence.Exceptions;
    using Webfarm.Sdk.Persistence.Impl;

    public class PersistenceCoreCompositionModule
    {
        public const string DefaultExceptionManager = "Persistence";

        public const string DefaultPolicy = "Persistence.Default";

        public PersistenceCoreCompositionModule(IContainer container)
        {
            var configuration = container.Resolve<IConfiguration>();
            var databaseOptions = configuration.Get<ApplicationDatabaseOptions>();

            container.UseInstance(databaseOptions);
            container.Register<IConnectionStringProvider, DefaultConnectionStringProvider>(Reuse.Singleton);
            container.Register<IConnectionFactory, PostgresConnectionFactory>(Reuse.Singleton/*, serviceKey: DatabaseType.Postgres*/);

            container.RegisterDelegate(this.BuildExceptionManager, Reuse.Singleton, serviceKey: DefaultExceptionManager);
        }

        [NotNull]
        protected virtual Type PostgresExceptionHandlerType => typeof(PostgresExceptionHandler);

        [NotNull]
        protected ExceptionManager BuildExceptionManager(IResolver resolver)
        {
            return BuildExceptionManager(this.GetExceptionPolicies);
        }

        protected void GetExceptionPolicies(ConfigurationSourceBuilder builder)
        {
            var policy = builder
                .ConfigureExceptionHandling()
                .GivenPolicyWithName(DefaultPolicy);

            this.ConfigureExceptionPolicy(policy);
        }

        protected virtual void ConfigureExceptionPolicy(IExceptionConfigurationForExceptionType policy)
        {
            policy
                .ForExceptionType<PostgresException>()
                    .HandleCustom(this.PostgresExceptionHandlerType)
                        .ThenThrowNewException()
                .ForExceptionType<Exception>()
                    .ThenNotifyRethrow();
        }

        [NotNull]
        private static ExceptionManager BuildExceptionManager([NotNull] Action<ConfigurationSourceBuilder> opt)
        {
            using (var configurationSource = new DictionaryConfigurationSource())
            {
                var builder = new ConfigurationSourceBuilder();
                opt(builder);
                builder.UpdateConfigurationWithReplace(configurationSource);

                var policyData = ExceptionHandlingSettings.GetExceptionHandlingSettings(configurationSource).ExceptionPolicies;
                var buildPolicies = policyData.Select(data => data.BuildExceptionPolicy());
                var manager = new ExceptionManager(buildPolicies);
                return manager;
            }
        }
    }
}
