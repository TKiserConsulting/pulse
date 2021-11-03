namespace Pulse.Api.Configuration
{
    using Microsoft.AspNetCore.SignalR;

    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var identity = connection.User?.Identity;
            return identity?.Name;
        }
    }
}
