namespace Infoplus.Askod.Sdk.Web.Api.Middlewares
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading.Tasks;
    using Infoplus.Askod.Sdk.Web.Api.Data;
    using Infoplus.Askod.Sdk.Web.Api.Exceptions;
    using Infoplus.Askod.Sdk.Web.Api.Extensions;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    public class ErrorHandlingApiMiddleware
    {
        #region Fields

        private readonly RequestDelegate next;

        private readonly ExceptionManager exceptionManager;
        private readonly IncludeErrorDetailPolicyType includeErrorDetailPolicy;

        #endregion Fields

        #region Constructor

        public ErrorHandlingApiMiddleware(RequestDelegate next, [NotNull] IOptions<ApplicationApiBehaviorOptions> options)
        {
            Contract.Assert(options != null);

            this.next = next;
            this.includeErrorDetailPolicy = options.Value.IncludeErrorDetailPolicy;
            this.exceptionManager = new ErrorHandlingService(this.includeErrorDetailPolicy).ExceptionManager;
        }

        #endregion Constructor

        #region Methods

        // ReSharper disable once UnusedMember.Global
        [DebuggerStepThrough]
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await this.next(context);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await this.HandleExceptionAsync(context, ex);
            }
        }

        [NotNull]
        private Task HandleExceptionAsync([NotNull] HttpContext context, Exception exception)
        {
            var transformedException =
                this.exceptionManager.TransformException(exception, ErrorHandlingService.DefaultExceptionPolicy);

            OperationError operationError;
            if (transformedException is OperationErrorApiException exc)
            {
                operationError = exc.Error;
            }
            else
            {
                operationError = new OperationError()
                    .With(HttpStatusCode.InternalServerError)
                    .With(transformedException ?? exception, this.includeErrorDetailPolicy);
            }

            return new OperationErrorResult(operationError).ExecuteResultAsync(context);
        }

        #endregion Methods
    }
}
