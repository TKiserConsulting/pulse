namespace Pulse.Api.Models.Users
{
    using System;
    using Data;
    using Data.Auth;
    using Webfarm.Sdk.Data;

    public class UserListItemDto : IIdentified<Guid>
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public UserRole Role { get; set; }
    }
}
