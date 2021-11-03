namespace Pulse.Api.Hubs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Persistence;
    using Webfarm.Sdk.Common.Extensions;

    [Authorize(Policy = "SignalRService")]
    public class InstructorHub : Hub<IInstructorHubClient>
    {
        private readonly PulseDbContext db;
        private readonly ILogger logger;

        public InstructorHub(PulseDbContext db, ILogger logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = this.Context.User?.UserId();
            // this.logger.LogDebug($"OnConnectedAsync, userId={userId}");

            if (userId != null)
            {
                var session = await this.db.Sessions.FirstOrDefaultAsync(m =>
                    m.Class.InstructorId == Guid.Parse(userId) && m.Finished == null);

                if (session != null)
                {
                    var group = session.SessionId.ToString();
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
                    // this.logger.LogDebug($"Added user '{userId}' to group '{group}");
                }
            }

            await base.OnConnectedAsync();
        }
    }
}
