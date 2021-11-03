namespace Webfarm.Sdk.Web.Api.Exceptions.Handlers
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Serilog;

    public class ApiLogWriteHandler : IExceptionHandler
    {
        public const string LevelKey = "Level";

        private static readonly ILogger Logger = Log.ForContext<ApiLogWriteHandler>();

        public ApiLogWriteHandler([NotNull] NameValueCollection collection)
        {
            Contract.Assert(collection != null);

            this.Level = TraceLevel.Error;
            if (collection.AllKeys.Contains(LevelKey))
            {
                this.Level = (TraceLevel)Enum.Parse(typeof(TraceLevel), collection[LevelKey]);
            }
        }

        public TraceLevel Level { get; set; }

        [NotNull]
        public Exception HandleException([NotNull] Exception exception, Guid handlingInstanceId)
        {
            Contract.Assert(exception != null);

            // var message = Format(exception, handlingInstanceId);
            const string messageTemplate =
                "Exception {ExceptionType} occurred: '{ExceptionMessage}' (handlingInstanceId {HandlingInstanceId})";
            var propertyValues = new object[]
            {
                exception.GetType().Name,
                exception.Message,
                handlingInstanceId
            };

            switch (this.Level)
            {
                case TraceLevel.Off:
                    break;
                case TraceLevel.Verbose:
                    Logger.Verbose(exception, messageTemplate, propertyValues);
                    break;
                case TraceLevel.Info:
                    Logger.Information(exception, messageTemplate, propertyValues);
                    break;
                case TraceLevel.Warning:
                    Logger.Warning(exception, messageTemplate, propertyValues);
                    break;
                case TraceLevel.Error:
                    Logger.Error(exception, messageTemplate, propertyValues);
                    break;
                default:
                    Logger.Error(exception, messageTemplate, propertyValues);
                    break;
            }

            return exception;
        }
    }
}
