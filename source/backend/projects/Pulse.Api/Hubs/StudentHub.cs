namespace Pulse.Api.Hubs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using Webfarm.Sdk.Common.Extensions;

    [Authorize(Policy = "SignalRService")]
    public class StudentHub : Hub<IStudentHubClient>
    {
        private readonly PulseDbContext db;

        public StudentHub(PulseDbContext db)
        {
            this.db = db;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = this.Context.User?.UserId();

            if (userId != null)
            {
                var sessionStudent = await this.db.SessionStudents.FirstOrDefaultAsync(m =>
                    m.SessionStudentId == Guid.Parse(userId));

                if (sessionStudent != null)
                {
                    var group = sessionStudent.SessionId.ToString();
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
                }
            }

            await base.OnConnectedAsync();
        }
    }
}
