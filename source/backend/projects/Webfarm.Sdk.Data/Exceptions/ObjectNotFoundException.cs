#pragma warning disable CA1032
namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class ObjectNotFoundException : ExecutionException
    {
        #region Constructors

        public ObjectNotFoundException()
            : this("Object cannot be found")
        {
        }

        public ObjectNotFoundException(string message, [CanBeNull] Exception innerException = null)
            : this(message, null, null, innerException)
        {
        }

        public ObjectNotFoundException(string id, string type, Exception ex)
            : this(ToMessage(id, type), id, type, ex)
        {
        }

        // ReSharper disable MemberCanBePrivate.Global
        public ObjectNotFoundException(string message, string id, string type, [CanBeNull] Exception ex = null)
            : base(message, WellKnownErrorSubcodeTypes.Default, ex)
        {
            // ReSharper restore MemberCanBePrivate.Global
            this.ErrorCode = WellKnownErrorCodeTypes.ResourceNotFound;
            this.ObjectId = id;
            this.ObjectType = type;
        }

        protected ObjectNotFoundException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ObjectId = info.GetString("ObjectId");
            this.ObjectType = info.GetString("ObjectType");
        }

        #endregion

        #region Properties

        // ReSharper disable MemberCanBePrivate.Global
        public string ObjectId { get; }

        public string ObjectType { get; }

        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        #region Methods

        // [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ObjectId", this.ObjectId);
            info.AddValue("ObjectType", this.ObjectType);
        }

        [NotNull]
        private static string ToMessage(string id, string type)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Can not find object {0} #{1}",
                type,
                id);
        }

        #endregion
    }
}
