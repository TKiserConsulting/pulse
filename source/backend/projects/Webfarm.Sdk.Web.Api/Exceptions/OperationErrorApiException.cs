namespace Webfarm.Sdk.Web.Api.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Web.Api.Data;

    [Serializable]
    public class OperationErrorApiException : ExecutionException
    {
        public OperationErrorApiException()
        {
        }

        public OperationErrorApiException(string message)
            : base(message)
        {
        }

        public OperationErrorApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OperationErrorApiException(OperationError error)
        {
            this.Error = error;
        }

        protected OperationErrorApiException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public OperationError Error { get; set; }
    }
}
