namespace Pulse.Api.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Managers;
    using Data;
    using Data.Auth;
    using Hubs;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Models.Sessions;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    public class CheckinController : Controller
    {
        private readonly PulseDbContext db;
        private readonly IMapper mapper;
        private readonly AuthManager authManager;
        private readonly IHubContext<StudentHub, IStudentHubClient> studentHub;

        public CheckinController(
            PulseDbContext db,
            IMapper mapper,
            AuthManager authManager,
            IHubContext<StudentHub, IStudentHubClient> studentHub)
        {
            this.db = db;
            this.mapper = mapper;
            this.authManager = authManager;
            this.studentHub = studentHub;
        }

        [HttpPost("checkin")]
        public async Task<SessionCheckinDetailsDto> Create(SessionCheckinCreateDto model, CancellationToken cancellationToken)
        {
            var session = await this.db.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    m => m.SessionId == model.SessionId && m.Class.InstructorId == this.authManager.UserId,
                    cancellationToken);

            if (session == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (session.Finished.HasValue)
            {
                return null;
            }

            var activeCheckin =
                await this.db.SessionCheckins
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.SessionId == model.SessionId && m.Finished == null, cancellationToken);

            if (activeCheckin != null)
            {
                return this.mapper.Map<SessionCheckinDetailsDto>(activeCheckin);
            }

            var instance = this.mapper.Map<SessionCheckin>(model);
            instance.SessionCheckinId = NewId.NextGuid();
            instance.Started = DateTimeOffset.Now;

            await this.db.SessionCheckins.AddAsync(instance, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);

            // send notification to the students
            var dto = this.mapper.Map<SessionCheckinDetailsDto>(instance);
            await this.studentHub.Clients.Group(instance.SessionId.ToString()).Checkin(dto);

            return dto;
        }

        [HttpPut("finish")]
        public async Task Finish(SessionCheckinFinishDto model, CancellationToken cancellationToken)
        {
            var instance = await this.db.SessionCheckins.FirstOrDefaultAsync(
                m => m.SessionCheckinId == model.SessionCheckinId &&
                     m.Session.Class.InstructorId == this.authManager.UserId, cancellationToken);

            if (instance == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (instance.Finished == null)
            {
                instance.Finished = DateTimeOffset.Now;
                this.db.SessionCheckins.Update(instance);
                await this.db.SaveChangesAsync(cancellationToken);
            }

            // send notification to the students
            await this.studentHub.Clients.Group(instance.SessionId.ToString()).CheckinFinish(instance.SessionCheckinId);
        }
    }
}
