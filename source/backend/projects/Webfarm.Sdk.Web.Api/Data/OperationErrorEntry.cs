namespace Webfarm.Sdk.Web.Api.Data
{
    using Newtonsoft.Json;

    public class OperationErrorEntry : IOperationExceptionDescription
    {
        [JsonProperty("code", Order = 5)]
        public string Code { get; set; }

        [JsonProperty("reason", Order = 10)]
        public string Reason { get; set; }

        [JsonProperty("exceptionMessage", Order = 20)]
        public string ExceptionMessage { get; set; }

        [JsonProperty("exceptionType", Order = 25)]
        public string ExceptionType { get; set; }

        [JsonProperty("stackTrace", Order = 30)]
        public string StackTrace { get; set; }
    }
}
