namespace Webfarm.Sdk.Web.Api.Filters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Webfarm.Sdk.Common.Transactions;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ApiTransactionAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync([NotNull] ActionExecutingContext actionContext, [NotNull] ActionExecutionDelegate next)
        {
            Contract.Assert(actionContext != null);
            Contract.Assert(next != null);

            var executeInTransaction = ShouldExecuteInTransaction(actionContext);
            var func = executeInTransaction
                ? (Func<ActionExecutionDelegate, Task>)this.ExecuteInTransaction
                : this.ExecuteDirect;

            await func(next);
        }

        private static bool ShouldExecuteInTransaction([NotNull] ActionContext actionContext)
        {
            var httpContext = actionContext.HttpContext;
            var transactionRequired =
                string.Equals(httpContext.Request.Method, HttpMethod.Delete.ToString(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(httpContext.Request.Method, HttpMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(httpContext.Request.Method, HttpMethod.Put.ToString(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(httpContext.Request.Method, HttpMethod.Patch.ToString(), StringComparison.OrdinalIgnoreCase);

            var executeInTransaction =
                transactionRequired &&
                actionContext.ActionDescriptor is ControllerActionDescriptor cad &&
                !cad.MethodInfo.GetCustomAttributes(typeof(CustomApiTransactionAttribute), true).Any();
            return executeInTransaction;
        }

        private async Task ExecuteInTransaction([NotNull] ActionExecutionDelegate next)
        {
            using (var transaction = new AppTransactionScope())
            {
                var actionExecutedContext = await this.ExecuteDirect(next);

                if (actionExecutedContext.Exception == null)
                {
                    transaction.Complete();
                }

                // rollback
            }
        }

        private async Task<ActionExecutedContext> ExecuteDirect([NotNull] ActionExecutionDelegate next)
        {
            var actionExecutedContext = await next();
            this.OnActionExecuted(actionExecutedContext);
            return actionExecutedContext;
        }
    }
}
