namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class ConcurrencyException : ExecutionException
    {
        public ConcurrencyException()
        {
            this.ErrorCode = WellKnownErrorCodeTypes.ConcurrencyViolation;
        }

        public ConcurrencyException(string message)
            : base(message, WellKnownErrorCodeTypes.ConcurrencyViolation)
        {
        }

        public ConcurrencyException(string message, Exception inner)
            : base(message, WellKnownErrorCodeTypes.ConcurrencyViolation, inner)
        {
        }

        protected ConcurrencyException(
            [NotNull] SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
