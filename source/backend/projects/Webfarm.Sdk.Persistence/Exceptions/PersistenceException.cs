namespace Webfarm.Sdk.Persistence.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class PersistenceException : ExecutionException
    {
        #region Constructors

        public PersistenceException()
        {
        }

        public PersistenceException(string message)
            : base(message)
        {
        }

        public PersistenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PersistenceException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.DatabaseType = (DatabaseType)info.GetInt32(nameof(this.DatabaseType));
        }

        #endregion

        public DatabaseType DatabaseType { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.DatabaseType), (int)this.DatabaseType);
        }
    }
}
