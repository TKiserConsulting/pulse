#pragma warning disable CA1032
namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class ExecutionException : RootException
    {
        #region Constructors and Destructors

        public ExecutionException()
            : this("Unexpected execution error.")
        {
        }

        public ExecutionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public ExecutionException(string message, string subcode = WellKnownErrorSubcodeTypes.Default, [CanBeNull] Exception inner = null)
            : base(message, inner)
        {
            this.ErrorCode = WellKnownErrorCodeTypes.OperationCannotBeFinished;
            this.ErrorSubcode = subcode;
        }

        protected ExecutionException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Contract.Assert(info != null);

            this.ErrorCode = info.GetString("Code");
            this.ErrorSubcode = info.GetString("Subcode");
            this.ErrorSource = info.GetString("ErrorSource");
        }

        #endregion

        #region Properties

        // ReSharper disable MemberCanBePrivate.Global
        public string ErrorCode { get; set; }

        public string ErrorSubcode { get; set; }

        public string ErrorSource { get; set; }

        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        #region Methods

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", this.ErrorCode);
            info.AddValue("Subcode", this.ErrorSubcode);
            info.AddValue("ErrorSource", this.ErrorSource);
        }

        #endregion
    }
}
