namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Webfarm.Sdk.Data.Metadata;

    [Serializable]
    public class ApplicationValidationException : ExecutionException
    {
        #region Fields

        private readonly IEnumerable<ApplicationValidationResult> errors;

        #endregion

        #region Constructors and Destructors

        public ApplicationValidationException()
            : this(string.Empty, "Unspecified validation failure.")
        {
        }

        public ApplicationValidationException(string message)
            : this(string.Empty, message)
        {
        }

        public ApplicationValidationException(string message, Exception inner)
            : this(string.Empty, null, message, inner)
        {
        }

        public ApplicationValidationException(
            string propertyName,
            [CanBeNull] string errorCode = null,
            [CanBeNull] string errorMessage = null,
            [CanBeNull] Exception innerException = null)
            : this(new[] { new ApplicationValidationResult(propertyName, errorCode, errorMessage) }, innerException)
        {
        }

        public ApplicationValidationException(params ApplicationValidationResult[] errors)
            : this((IList<ApplicationValidationResult>)errors)
        {
        }

        public ApplicationValidationException(IList<ApplicationValidationResult> errors, [CanBeNull] Exception innerException = null)
            : base(BuildErrorMessage(errors), inner: innerException)
        {
            // ReSharper disable PossibleMultipleEnumeration
            this.ErrorCode = WellKnownErrorCodeTypes.InvalidData;
            this.errors = errors;

            // ReSharper restore PossibleMultipleEnumeration
        }

        protected ApplicationValidationException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Properties

        [NotNull]
        public IEnumerable<ApplicationValidationResult> Errors => this.errors ?? Array.Empty<ApplicationValidationResult>();

        #endregion

        #region Methods

        [NotNull]
        private static string BuildErrorMessage(IEnumerable<ApplicationValidationResult> errors)
        {
            var arr = (from x in errors where x?.ErrorMessage != null select "\r\n -- " + x.ErrorMessage).ToArray();
            return "Validation failed: " + string.Join(string.Empty, arr);
        }

        #endregion
    }
}
