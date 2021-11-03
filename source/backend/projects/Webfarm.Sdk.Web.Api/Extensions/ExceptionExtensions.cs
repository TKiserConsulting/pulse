namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Webfarm.Sdk.Web.Api.Exceptions.Handlers;

    public static class ExceptionExtensions
    {
        [CanBeNull]
        public static Exception TransformException([NotNull] this ExceptionManager exceptionManager, [CanBeNull] Exception ex, [NotNull] string policyName)
        {
            Contract.Assert(exceptionManager != null);
            Contract.Assert(policyName != null);

            var transformedException = ex;
            if (ex != null)
            {
                var rethrow = exceptionManager.HandleException(ex, policyName, out var exceptionToThrow);
                if (rethrow && exceptionToThrow != null)
                {
                    transformedException = exceptionToThrow;
                }
            }

            return transformedException;
        }

        public static IExceptionConfigurationForExceptionType LogThenThrowNewException<TException, TExceptionHandler>(
            [NotNull] this IExceptionConfigurationForExceptionType policyBuilder,
            NameValueCollection loggingSettings,
            [CanBeNull] NameValueCollection handlingSettings = null)
            where TException : Exception
            where TExceptionHandler : IExceptionHandler
        {
            Contract.Assert(policyBuilder != null);

            policyBuilder = policyBuilder.ForExceptionType<TException>()
                .HandleCustom(typeof(ApiLogWriteHandler), loggingSettings)
                .HandleCustom(typeof(TExceptionHandler), handlingSettings ?? new NameValueCollection())
                .ThenThrowNewException();
            return policyBuilder;
        }
    }
}
