namespace Webfarm.Sdk.Web.Api.Data
{
    public interface IOperationExceptionDescription
    {
        string ExceptionMessage { get; set; }

        string ExceptionType { get; set; }

        string StackTrace { get; set; }
    }
}
