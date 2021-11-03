namespace Webfarm.Sdk.Persistence.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class PersistenceConstraintException : PersistenceException
    {
        #region Constructors

        public PersistenceConstraintException()
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            : this(@"Operation constraint occured")
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        {
        }

        public PersistenceConstraintException(string message)
            : this(message, null)
        {
        }

        public PersistenceConstraintException(Exception innerException)
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            : this(@"Operation constraint occured", innerException)
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        {
        }

        public PersistenceConstraintException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = WellKnownErrorCodeTypes.OperationConstraint;
        }

        protected PersistenceConstraintException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ConstraintName = info.GetString(nameof(this.ConstraintName));
            this.TableName = info.GetString(nameof(this.TableName));
        }

        #endregion

        public string ConstraintName { get; set; }

        public string TableName { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.ConstraintName), this.ConstraintName);
            info.AddValue(nameof(this.TableName), this.TableName);
        }
    }
}
