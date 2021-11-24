namespace Pulse.Api.Managers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Persistence;

    public class SessionManager
    {
        private readonly PulseDbContext db;

        public SessionManager(
            PulseDbContext db)
        {
            this.db = db;
        }

        public async Task<Session> FinishExpiredSession(Session session, Guid instructorId, CancellationToken cancellationToken)
        {
            var settings = await this.db.InstructorSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.InstructorId == instructorId, cancellationToken);

            var timeoutHours = settings?.SessionTimeoutHours ?? InstructorSettings.DefaultSessionTimeoutHours;

            if ((decimal)(DateTimeOffset.Now - session.Created).TotalHours >= timeoutHours)
            {
                session.Finished = DateTimeOffset.Now;
                await this.db.SaveChangesAsync(cancellationToken);
                return null;
            }

            return session;
        }
    }
}
