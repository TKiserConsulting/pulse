namespace Webfarm.Sdk.Data.Exceptions
{
    using System;
    using JetBrains.Annotations;

    [Serializable]
    public class ApplicationValidationResult
    {
        [NonSerialized]
        private readonly object attemptedValue;

        [NonSerialized]
        private readonly object customState;

        #region Constructors and Destructors

        public ApplicationValidationResult(
            string propertyName,
            [CanBeNull] string errorCode = null,
            [CanBeNull] string error = null,
            [CanBeNull] object value = null,
            [CanBeNull] object customState = null)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = error;
            this.ErrorCode = errorCode;
            this.attemptedValue = value;
            this.customState = customState;
        }

        #endregion

        #region Public Properties

        public object AttemptedValue => this.attemptedValue;

        public object CustomState => this.customState;

        public string ErrorCode { get; }

        public string ErrorMessage { get; }

        public string PropertyName { get; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.ErrorMessage;
        }

        #endregion
    }
}
