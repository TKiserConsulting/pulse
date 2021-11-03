namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    using JetBrains.Annotations;

    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class UnauthenticatedException : ExecutionException
    {
        public UnauthenticatedException()
        {
            this.ErrorCode = WellKnownErrorCodeTypes.Unauthenticated;
        }

        public UnauthenticatedException(string message)
            : base(message, WellKnownErrorCodeTypes.Unauthenticated)
        {
        }

        public UnauthenticatedException(string message, Exception inner)
            : base(message, WellKnownErrorCodeTypes.ConcurrencyViolation, inner)
        {
        }

        protected UnauthenticatedException(
            [NotNull] SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
