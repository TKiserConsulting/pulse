#pragma warning disable CA1032
namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;

    [Serializable]
    public abstract class RootException : Exception
    {
        #region Constructors

        protected RootException()
            : this(null)
        {
        }

        protected RootException(string message, [CanBeNull] Exception innerException = null)
            : base(message, innerException)
        {
        }

        protected RootException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
