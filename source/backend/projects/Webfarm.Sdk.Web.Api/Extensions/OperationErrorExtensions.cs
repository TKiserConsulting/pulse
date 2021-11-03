namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Data.Metadata;
    using Webfarm.Sdk.Web.Api.Data;

    public static class OperationErrorExtensions
    {
        public static readonly Tuple<HttpStatusCode, string> UnexpectedError =
            Tuple.Create(HttpStatusCode.Gone, "Unexpected error occurred.");

        public static readonly Dictionary<string, string> ValidationMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "NotEmptyValidator", WellKnownErrorSubcodeTypes.Required }
        };

        public static readonly Dictionary<string, Tuple<HttpStatusCode, string>> StatusCodeMap = new Dictionary<string, Tuple<HttpStatusCode, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { WellKnownErrorCodeTypes.Unauthenticated, Tuple.Create(HttpStatusCode.Unauthorized, "Request not authenticated.") },
            { WellKnownErrorCodeTypes.Forbidden, Tuple.Create(HttpStatusCode.Forbidden, "Request not authorized.") },
            { WellKnownErrorCodeTypes.ResourceNotFound, Tuple.Create(HttpStatusCode.NotFound, "Requested resource not found.") },
            { WellKnownErrorCodeTypes.InvalidData, Tuple.Create(HttpStatusCode.Conflict, "Incoming data has validation implications.") },
            { WellKnownErrorCodeTypes.ConcurrencyViolation, Tuple.Create(HttpStatusCode.NotAcceptable, "Operation is locked due to concurrency.") },
            { WellKnownErrorCodeTypes.OperationConstraint, Tuple.Create(HttpStatusCode.Conflict, "Operation restricted.") },
            { WellKnownErrorCodeTypes.OperationCannotBeFinished, Tuple.Create(HttpStatusCode.Conflict, "Operation cannot be finished.") },
            { WellKnownErrorCodeTypes.UnexpectedError, Tuple.Create(HttpStatusCode.InternalServerError, "Unexpected error occurred.") },
            { ErrorCodes.UnsupportedApiVersion, Tuple.Create(HttpStatusCode.NotFound, "Unsupported api version.") },
            { ErrorCodes.AmbiguousApiVersion, Tuple.Create(HttpStatusCode.BadRequest, "Unexpected error occurred.") },
            { ErrorCodes.ApiVersionUnspecified, Tuple.Create(HttpStatusCode.BadRequest, "Version unspecified.") },
            { ErrorCodes.InvalidApiVersion, Tuple.Create(HttpStatusCode.BadRequest, "Invalid version.") },
        };

        public static readonly Dictionary<string, string> ModelStateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(RequiredAttribute), WellKnownErrorSubcodeTypes.Required },
            { nameof(RangeAttribute), WellKnownErrorSubcodeTypes.Invalid },
            { nameof(RegularExpressionAttribute), WellKnownErrorSubcodeTypes.InvalidFormat },
            { nameof(MaxLengthAttribute), WellKnownErrorSubcodeTypes.MaxLengthExceeded },
            { nameof(MinLengthAttribute), WellKnownErrorSubcodeTypes.Invalid },
            { nameof(StringLengthAttribute), WellKnownErrorSubcodeTypes.Invalid },
        };

        [NotNull]
        public static OperationError With([NotNull] this OperationError error, [NotNull] string code, [CanBeNull] string subcode = null, [CanBeNull] string instance = null)
        {
            Contract.Assert(error != null);
            Contract.Assert(code != null);

            error.Status = ResolveStatus(code);
            error.Title = ResolveMessage(code);
            error.Code = code.ToString(CultureInfo.InvariantCulture);
            error.Subcode = subcode?.ToString(CultureInfo.InvariantCulture);
            error.Instance = instance;

            return error;
        }

#pragma warning disable CA1506
        public static OperationError With(
            this OperationError error,
            ModelStateDictionary modelState,
            IncludeErrorDetailPolicyType exceptionPolicy)
#pragma warning restore CA1506
        {
            var r = new Regex(@"^\[(?<code>.*)\] (?<message>.*)", RegexOptions.Singleline);

            var items = from entry in modelState
                where entry.Value.Errors.Count > 0
                from e in entry.Value.Errors
                let m = r.Match(e.ErrorMessage ?? string.Empty)
                let sourceCode = m.Success ? m.Groups["code"].Value : string.Empty
                let code = !string.IsNullOrEmpty(sourceCode) && ModelStateMap.ContainsKey(sourceCode) ? ModelStateMap[sourceCode] : WellKnownErrorSubcodeTypes.Default
                let message = m.Success ? m.Groups["message"].Value : e.ErrorMessage
                select Tuple.Create(entry.Key, message, code, e.Exception);
            items.ForEach(item => error.AddError(item.Item1, item.Item3, item.Item2, exceptionPolicy, item.Item4));

            return error;
        }

        [NotNull]
        public static OperationError WithValidation(
            [NotNull] this OperationError error,
            [NotNull] ApplicationValidationException exception,
            IncludeErrorDetailPolicyType exceptionPolicy)
        {
            Contract.Assert(exception != null);
            error
                .With(exception.ErrorCode)
                .With(exception, exceptionPolicy);

            exception.Errors.ForEach(e =>
            {
                var code = ValidationMap.ContainsKey(e.ErrorCode) ? ValidationMap[e.ErrorCode] : e.ErrorCode;
                error.AddError(e.PropertyName, code, e.ErrorMessage);
            });

            return error;
        }

        [NotNull]
        public static OperationError With([NotNull] this OperationError error, HttpStatusCode statusCode)
        {
            Contract.Assert(error != null);

            error.Status = statusCode;
            return error;
        }

        public static T With<T>(this T destination, [CanBeNull] Exception exception, IncludeErrorDetailPolicyType policy)
            where T : IOperationExceptionDescription
        {
            if (exception != null && policy == IncludeErrorDetailPolicyType.Include)
            {
                destination.ExceptionMessage = exception.Message;
                destination.ExceptionType = exception.GetType().Name;
                destination.StackTrace = exception.StackTrace;
            }

            return destination;
        }

        public static OperationError Build([NotNull] string code, Exception exception, IncludeErrorDetailPolicyType policy, [CanBeNull] string subcode = null)
        {
            var operationError = new OperationError()
                .With(code, subcode)
                .With(exception, policy);
            return operationError;
        }

        public static async Task SendOperationErrorAsResult([NotNull] this JwtBearerChallengeContext context)
        {
            Contract.Assert(context != null);

            await new OperationError()
                .With(WellKnownErrorCodeTypes.Unauthenticated)
                .WriteTo(context.Response);

            context.HandleResponse();
        }

        public static async Task SendOperationErrorAsResult([NotNull] this ForbiddenContext context)
        {
            Contract.Assert(context != null);

            await new OperationError()
                .With(WellKnownErrorCodeTypes.Forbidden)
                .WriteTo(context.Response);
        }

        public static async Task SendOperationErrorAsResult([NotNull] this AuthenticationFailedContext context)
        {
            Contract.Assert(context != null);

            context.NoResult();

            await new OperationError()
                .With(WellKnownErrorCodeTypes.Unauthenticated)
                .WriteTo(context.Response);
        }

        public static async Task WriteTo([NotNull] this OperationError operationError, [NotNull] HttpResponse response)
        {
            Contract.Assert(operationError != null);
            Contract.Assert(response != null);

            response.StatusCode = (int)(operationError.Status ?? HttpStatusCode.InternalServerError);
            response.ContentType = OperationError.ErrorContentType;
            var result = operationError.Serialize();
            await response.WriteAsync(result);
        }

        private static HttpStatusCode ResolveStatus([NotNull] string code)
        {
            var mapEntry = GetMapEntry(code);
            return mapEntry.Item1;
        }

        private static string ResolveMessage([NotNull] string code)
        {
            var (_, message) = GetMapEntry(code);
            return message;
        }

        private static (HttpStatusCode, string) GetMapEntry([NotNull] string code)
        {
            var (statusCode, message) = StatusCodeMap.ContainsKey(code) ? StatusCodeMap[code] : UnexpectedError;
            return (statusCode, message);
        }
    }
}
