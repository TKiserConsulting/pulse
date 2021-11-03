namespace Webfarm.Sdk.Web.Api.Middlewares
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using DryIoc;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Webfarm.Sdk.Common;

    public class ExecutionContextTraceIdentifierBinderMiddleware
    {
        #region Fields

        private readonly RequestDelegate next;

        #endregion Fields

        #region Constructor

        public ExecutionContextTraceIdentifierBinderMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        #endregion Constructor

        #region Methods

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke([NotNull] HttpContext context)
        {
            Contract.Assert(context != null);
            Debug.Assert(context.RequestServices != null, "context.RequestServices != null");
            var resolver = (IResolver)context.RequestServices.GetService(typeof(IResolver));
            var executionContext = resolver.Resolve<IExecutionContext>();
            var trackingId = context.TraceIdentifier;
            executionContext.Data.Add(nameof(trackingId), trackingId);

            await this.next(context);
        }

        #endregion Methods
    }
}
