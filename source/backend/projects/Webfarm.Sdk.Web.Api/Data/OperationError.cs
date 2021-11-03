namespace Webfarm.Sdk.Web.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Webfarm.Sdk.Web.Api.Extensions;

    // https://tools.ietf.org/html/rfc7807
    [Serializable]
    public class OperationError : IOperationExceptionDescription
    {
        public const string ErrorContentType = "application/problem+json";

        [NonSerialized]
        private Dictionary<string, List<OperationErrorEntry>> errors;

        [NonSerialized]
        private Dictionary<string, string> data;

        [NonSerialized]
        private Guid? handlingInstanceId;

        [NonSerialized]
        private HttpStatusCode? status;

        public OperationError()
        {
            this.Status = HttpStatusCode.InternalServerError;
        }

        [JsonProperty("status", Order = 1)]
        public HttpStatusCode? Status
        {
            get => this.status;
            set => this.status = value;
        }

        /*
        [JsonProperty("type", Order = 5)]
        public string Type =>
            string.IsNullOrWhiteSpace(this.Subcode) ?
                $"{RfcTypePrefix}/{this.Code}" :
                $"{RfcTypePrefix}/{this.Subcode}";
        */

        [JsonProperty("code", Order = 6)]
        public string Code { get; set; }

        [JsonProperty("subcode", Order = 7)]
        public string Subcode { get; set; }

        [JsonProperty("title", Order = 10)]
        public string Title { get; set; }

        [JsonProperty("detail", Order = 11)]
        public string Detail { get; set; }

        [JsonProperty("data", Order = 30)]
        public Dictionary<string, string> Data
        {
            get => this.data;
            private set => this.data = value;
        }

        [JsonProperty("errors", Order = 35)]
        public Dictionary<string, List<OperationErrorEntry>> Errors
        {
            get => this.errors;
            private set => this.errors = value;
        }

        [JsonProperty("instance", Order = 40)]
        public string Instance { get; set; }

        [JsonProperty("handlingInstanceId", Order = 45)]
        public Guid? HandlingInstanceId
        {
            get => this.handlingInstanceId;
            set => this.handlingInstanceId = value;
        }

        [JsonProperty("exceptionMessage", Order = 50)]
        public string ExceptionMessage { get; set; }

        [JsonProperty("exceptionType", Order = 55)]
        public string ExceptionType { get; set; }

        [JsonProperty("stackTrace", Order = 60)]
        public string StackTrace { get; set; }

        [NotNull]
        public OperationErrorEntry AddError(
            [NotNull] string name,
            [CanBeNull] string code,
            string reason,
            IncludeErrorDetailPolicyType exceptionPolicy = IncludeErrorDetailPolicyType.Omit,
            [CanBeNull] Exception exception = null)
        {
            Contract.Assert(name != null);

            if (this.Errors == null)
            {
                this.Errors = new Dictionary<string, List<OperationErrorEntry>>();
            }

            if (name.Length > 0)
            {
                // transform first char to lower to match UI
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            }

            if (!this.Errors.ContainsKey(name))
            {
                this.Errors.Add(name, new List<OperationErrorEntry>());
            }

            var list = this.Errors[name];

            var entry = new OperationErrorEntry
                            {
                                Code = string.IsNullOrEmpty(code) ? null : code,
                                Reason = reason
                            };
            entry.With(exception, exceptionPolicy);

            list.Add(entry);

            return entry;
        }

        public void AddData([NotNull] string key, string value)
        {
            Contract.Assert(key != null);

            if (this.Data == null)
            {
                this.Data = new Dictionary<string, string>();
            }

            this.Data.Add(key, value);
        }
    }
}
