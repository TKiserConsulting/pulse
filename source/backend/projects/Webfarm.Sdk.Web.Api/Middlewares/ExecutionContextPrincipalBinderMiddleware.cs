namespace Webfarm.Sdk.Web.Api.Middlewares
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;

    using Webfarm.Sdk.Web.Api.Extensions;

    public class ExecutionContextPrincipalBinderMiddleware
    {
        #region Fields

        private readonly RequestDelegate next;

        #endregion Fields

        #region Constructor

        public ExecutionContextPrincipalBinderMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        #endregion Constructor

        #region Methods

        // ReSharper disable once UnusedMember.Global
        [DebuggerStepThrough]
        public async Task Invoke([NotNull] HttpContext context /* other dependencies */)
        {
            context.RegisterPrincipal(context.User);

            await this.next(context);
        }

        #endregion Methods
    }
}
