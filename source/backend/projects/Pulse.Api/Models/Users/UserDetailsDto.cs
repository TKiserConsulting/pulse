namespace Pulse.Api.Models.Users
{
    using System;
    using Data.Auth;
    using Webfarm.Sdk.Data;

    public class UserDetailsDto : IIdentified<Guid>
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string School { get; set; }

        public string Subject { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public UserRole Role { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset Created { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }
    }
}
