namespace Pulse.Api.Common.Managers
{
    using System;
    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Common.Extensions;

    public class AuthManager
    {
        private readonly IExecutionContext executionContext;

        public AuthManager(IExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public Guid UserId => Guid.Parse(this.executionContext.UserId());
    }
}
