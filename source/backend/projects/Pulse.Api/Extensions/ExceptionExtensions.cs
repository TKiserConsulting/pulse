namespace Pulse.Api.Extensions
{
    using System;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Persistence.Exceptions;

    public static class ExceptionExtensions
    {
        public static Exception WrapPersistenceUniqueness(this Exception sourceException, string propertyName, Func<PersistenceConstraintException, bool> condition = null)
        {
            condition ??= ex => true;
            return sourceException.WrapPersistenceConstraint(propertyName, WellKnownErrorSubcodeTypes.Duplicated, condition);
        }

        public static Exception WrapPersistenceConstraint(this Exception sourceException, string propertyName, string code, Func<PersistenceConstraintException, bool> condition = null)
        {
            Exception mappedException = null;
            condition ??= ex => true;
            if (sourceException is PersistenceConstraintException pex && condition(pex))
            {
                mappedException = new ApplicationValidationException(
                    propertyName,
                    code,
                    innerException: sourceException);
            }

            return mappedException;
        }
    }
}
