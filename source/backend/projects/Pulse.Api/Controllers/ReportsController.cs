namespace Pulse.Api.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Managers;
    using Data.Auth;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Classes;
    using Models.Emoticons;
    using Models.Reports;
    using Models.Sessions;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;
    using Webfarm.Sdk.Data.Exceptions;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly PulseDbContext db;
        private readonly AuthManager authManager;
        private readonly IMapper mapper;

        public ReportsController(
            PulseDbContext db,
            AuthManager authManager,
            IMapper mapper)
        {
            this.db = db;
            this.authManager = authManager;
            this.mapper = mapper;
        }

        [HttpGet("session")]
        public async Task<SessionReportResultDto> SessionReport([FromQuery][Required] Guid sessionId, CancellationToken cancellationToken)
        {
            var session = await this.db.Sessions
                .Include(m => m.Class)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.SessionId == sessionId, cancellationToken);

            if (session == null)
            {
                throw new ApplicationValidationException($"Session '{sessionId}' does not exist");
            }

            if (session.Class.InstructorId != this.authManager.UserId)
            {
                throw new UnauthorizedAccessException($"User not permitted to access session {sessionId}");
            }

            var result = new SessionReportResultDto
            {
                Class = this.mapper.Map<ClassListItemDto>(session.Class),
                Session = this.mapper.Map<SessionListItemDto>(session),
                IntervalMinutes = 5
            };

            var checkins = await this.db.SessionCheckins
                .Where(m => m.SessionId == sessionId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var questions = await this.db.SessionQuestions
                .Where(m => m.SessionCheckin.SessionId == sessionId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            result.Checkins = checkins.Select(m => new CheckinData
            {
                Checkin = this.mapper.Map<SessionCheckinDetailsDto>(m),
                Questions = this.mapper.Map<SessionQuestionDetailsDto[]>(
                    questions.Where(q => q.SessionCheckinId == m.SessionCheckinId))
            }).ToArray();

            var interval = new TimeSpan(0, result.IntervalMinutes, 0);  // 5 minutes

            var taps = await this.db.EmoticonTaps
                .Where(m => m.SessionStudent.SessionId == sessionId)
                .GroupBy(m => new { m.InstructorEmoticonId, Timestamp = m.TimeTicks / interval.Ticks })
                .Select(g => new { g.Key.InstructorEmoticonId, g.Key.Timestamp, TapCount = g.Count() })
                .OrderBy(m => m.InstructorEmoticonId)
                .ThenBy(m => m.Timestamp)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            //var tapsQuery = from tap in this.db.EmoticonTaps
            //    group tap by tap.Created.Ticks / interval.Ticks
            //    into g
            //    select new { Timestamp = new DateTime(g.Key * interval.Ticks), TapCount = g.Count() };

            var emoticons = await this.db.InstructorEmoticons
                .Where(m => m.InstructorId == this.authManager.UserId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            result.Emoticons = emoticons.Select(m => new EmoticonData
            {
                Emoticon = this.mapper.Map<InstructorEmoticonListItemDto>(m),
                Taps = taps
                    .Where(t => t.InstructorEmoticonId == m.InstructorEmoticonId)
                    .Select(t => new EmoticonTapData
                    {
                        Timestamp = new DateTimeOffset(t.Timestamp * interval.Ticks,
                            new TimeSpan(session.Created.Offset.Ticks)),
                        TapCount = t.TapCount
                    }).ToArray()
            }).ToArray();

            return result;
        }

    }
}
