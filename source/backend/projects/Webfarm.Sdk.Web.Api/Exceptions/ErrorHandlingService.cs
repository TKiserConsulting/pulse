namespace Webfarm.Sdk.Web.Api.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Security;
    using JetBrains.Annotations;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Exceptions.Handlers;
    using Webfarm.Sdk.Web.Api.Extensions;

    public class ErrorHandlingService
    {
        #region Fields

        public const string DefaultExceptionPolicy = "Communication.Json";

        public const string TransactionRollbackExceptionPolicy = "Communication.Rollback";

        private readonly NameValueCollection defaultHandlingSettings =
            new NameValueCollection();

        private readonly NameValueCollection unexpectedHandlingSettings =
            new NameValueCollection
            {
                {
                    WrapHandler<Exception>.MessageKey,
                    "Unexpected error occured while processing your request. Please, contact administrator for details. [Error ID: {handlingInstanceID}]"
                },
                { WrapHandler<Exception>.AppendHandlingIdKey, bool.TrueString }
            };

        private readonly NameValueCollection traceLevelSettings =
            new NameValueCollection { { ApiLogWriteHandler.LevelKey, TraceLevel.Verbose.ToString("G") } };

        private readonly NameValueCollection infoLevelSettings =
            new NameValueCollection { { ApiLogWriteHandler.LevelKey, TraceLevel.Info.ToString("G") } };

        private readonly NameValueCollection warnLevelSettings =
            new NameValueCollection { { ApiLogWriteHandler.LevelKey, TraceLevel.Warning.ToString("G") } };

        private readonly NameValueCollection fatalLevelSettings =
            new NameValueCollection { { ApiLogWriteHandler.LevelKey, TraceLevel.Error.ToString("G") } };

        #endregion

        public ErrorHandlingService(IncludeErrorDetailPolicyType exceptionPolicy)
        {
            this.defaultHandlingSettings.Add(WrapHandler<Exception>.PolicyKey, exceptionPolicy.ToString("G"));
            this.unexpectedHandlingSettings.Add(WrapHandler<Exception>.PolicyKey, exceptionPolicy.ToString("G"));

            this.ExceptionManager = this.BuildExceptionManager();
        }

        public ExceptionManager ExceptionManager { get; }

        #region ExceptionHandling

        protected virtual IExceptionConfigurationForExceptionType ConfigureDefaultPolicyExceptionHandling(
            IExceptionConfigurationForExceptionType policyBuilder)
        {
            policyBuilder = policyBuilder
                .LogThenThrowNewException<ApplicationValidationException, ValidationExceptionHandler>(this.traceLevelSettings, this.defaultHandlingSettings)
                .LogThenThrowNewException<ExecutionException, ExecutionExceptionHandler>(this.traceLevelSettings, this.defaultHandlingSettings)
                .LogThenThrowNewException<ObjectNotFoundException, ObjectNotFoundExceptionHandler>(this.infoLevelSettings, this.defaultHandlingSettings)
                .LogThenThrowNewException<UnauthenticatedException, UnauthenticatedExceptionHandler<UnauthenticatedException>>(this.warnLevelSettings, this.defaultHandlingSettings)
                .LogThenThrowNewException<UnauthorizedAccessException, AccessDeniedExceptionHandler<UnauthorizedAccessException>>(
                    this.warnLevelSettings,
                    this.unexpectedHandlingSettings)
                .LogThenThrowNewException<SecurityException, AccessDeniedExceptionHandler<SecurityException>>(
                    this.warnLevelSettings,
                    this.unexpectedHandlingSettings)
                .LogThenThrowNewException<Exception, UnexpectedExceptionHandler>(
                    this.fatalLevelSettings,
                    this.unexpectedHandlingSettings);
            return policyBuilder;
        }

        protected virtual IExceptionConfigurationForExceptionType ConfigureTransactionPolicyExceptionHandling(
            [NotNull] IExceptionConfigurationForExceptionType policyBuilder)
        {
            return policyBuilder
                .LogThenThrowNewException<Exception, UnexpectedExceptionHandler>(
                    this.fatalLevelSettings,
                    this.unexpectedHandlingSettings);
        }

        [NotNull]
        private ExceptionManager BuildExceptionManager()
        {
            var policies = this.GetExceptionPolicyDataList().Select(data => data.BuildExceptionPolicy());
            var manager = new ExceptionManager(policies);
            return manager;
        }

        private IEnumerable<ExceptionPolicyData> GetExceptionPolicyDataList()
        {
            using (var configurationSource = new DictionaryConfigurationSource())
            {
                var builder = new ConfigurationSourceBuilder();

                var policyBuilder = builder.ConfigureExceptionHandling()
                    .GivenPolicyWithName(DefaultExceptionPolicy);
                policyBuilder = this.ConfigureDefaultPolicyExceptionHandling(policyBuilder);

                policyBuilder = policyBuilder
                    .GivenPolicyWithName(TransactionRollbackExceptionPolicy);
                this.ConfigureTransactionPolicyExceptionHandling(policyBuilder);

                builder.UpdateConfigurationWithReplace(configurationSource);

                return ExceptionHandlingSettings.GetExceptionHandlingSettings(configurationSource).ExceptionPolicies;
            }
        }

        #endregion
    }
}
