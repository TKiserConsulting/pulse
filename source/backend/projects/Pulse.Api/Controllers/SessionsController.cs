namespace Pulse.Api.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Common.Managers;
    using Data;
    using Data.Auth;
    using Extensions;
    using Hubs;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Models.Sessions;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Web.Api.Data;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    public class SessionsController : BaseDbCrudController<Session, SessionListItemDto, SessionDetailsDto, SessionCreateDto, SessionCreateDto>
    {
        private readonly AuthManager authManager;
        private readonly IHubContext<StudentHub, IStudentHubClient> studentHub;

        public SessionsController(
            PulseDbContext db,
            IMapper mapper,
            AuthManager authManager,
            IHubContext<StudentHub, IStudentHubClient> studentHub)
            : base(db, mapper)
        {
            this.authManager = authManager;
            this.studentHub = studentHub;
        }

        [HttpGet("activesession")]
        public async Task<ActiveSessionDetailsDto> GetActiveSession(CancellationToken cancellationToken)
        {
            var instance = await this.Db.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Class.InstructorId == this.authManager.UserId && m.Finished == null,
                    cancellationToken);

            if (instance == null)
            {
                return null;
            }

            var model = this.Mapper.Map<ActiveSessionDetailsDto>(instance);

            var emoticons = await this.Db.InstructorEmoticons
                .Where(m => m.InstructorId == this.authManager.UserId)
                .OrderBy(m => m.SortIndex)
                .ThenBy(m => m.Created)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            model.Emoticons = this.Mapper.Map<SessionEmoticonDetailsDto[]>(emoticons);

            var tapsQuery = from m in this.Db.EmoticonTaps
                where m.SessionStudent.SessionId == instance.SessionId
                group m by m.InstructorEmoticonId
                into g
                select new { InstructorEmoticonId = g.Key, Count = g.Count() };

            var taps = await tapsQuery
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var tap in taps)
            {
                var emoticon = model.Emoticons.FirstOrDefault(e => e.InstructorEmoticonId == tap.InstructorEmoticonId);
                if (emoticon != null)
                {
                    emoticon.TapCount = tap.Count;
                }
            }

            var checkin =
                await this.Db.SessionCheckins
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.SessionId == instance.SessionId && m.Finished == null,
                        cancellationToken);

            if (checkin != null)
            {
                model.ActiveCheckin = this.Mapper.Map<SessionCheckinDetailsDto>(checkin);

                var questions = await this.Db.SessionQuestions
                    .Where(m => m.SessionCheckinId == checkin.SessionCheckinId && m.Dismissed == null)
                    .OrderBy(m => m.Created)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                model.ActiveCheckin.Questions = this.Mapper.Map<SessionQuestionDetailsDto[]>(questions);
            }

            return model;
        }

        [HttpPost("endsession")]
        public async Task EndSession(CancellationToken cancellationToken)
        {
            var instance = await this.Db.Sessions.FirstOrDefaultAsync(
                m => m.Class.InstructorId == this.authManager.UserId && m.Finished == null, cancellationToken);

            if (instance != null)
            {
                instance.Finished = DateTimeOffset.Now;
                this.Db.Sessions.Update(instance);
                await this.Db.SaveChangesAsync(cancellationToken);

                await this.studentHub.Clients.Group(instance.SessionId.ToString())
                    .SessionFinish(instance.SessionId);
            }
        }

        protected override IQueryable<Session> OrderOnFind(IQueryable<Session> query)
        {
            return query.OrderBy(m => m.Created);
        }

        protected override IQueryable<Session> Filter(IQueryable<Session> q, FilteringData filterBy)
        {
            if (!filterBy.TryGetValue("classId", out var classId))
            {
                throw new ApplicationValidationException("Parameter 'classId' is required");
            }

            var theClass = this.Db.Classes
                .AsNoTracking()
                .FirstOrDefault(m => m.ClassId == Guid.Parse(classId));

            if (theClass == null)
            {
                throw new ApplicationValidationException($"Class '{classId}' does not exist");
            }

            if (theClass.InstructorId != this.authManager.UserId)
            {
                throw new UnauthorizedAccessException($"User not permitted to access class {classId}");
            }

            q = q.Where(m => m.ClassId == theClass.Id);

            return q;
        }

        protected override async Task<Session> MapOnCreate(SessionCreateDto model, CancellationToken cancellationToken)
        {
            var instance = await base.MapOnCreate(model, cancellationToken);
            while (true)
            {
                instance.Code = this.GenerateSessionCode(instance.Id);
                var existing = await this.Db.Sessions
                    .FirstOrDefaultAsync(
                        m => m.Code == instance.Code && m.Finished != null,
                        cancellationToken);
                if (existing == null)
                {
                    break;
                }
                else
                {
                    instance.Id = NewId.NextGuid();
                }
            }
            return instance;
        }

        protected override Exception MapException(Exception sourceException)
        {
            return sourceException.WrapPersistenceUniqueness(nameof(Session.SessionId));
        }

        private string GenerateSessionCode(Guid id)
        {
            const int CodeLength = 6;
            return Math.Abs(id.ToString().GetHashCode())
                .ToString(CultureInfo.InvariantCulture)
                .PadRight(CodeLength, '0')
                .Substring(0, CodeLength);
        }
    }
}
