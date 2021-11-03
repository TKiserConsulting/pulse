namespace Pulse.Api.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Managers;
    using Data.Auth;
    using Hubs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Models.Sessions;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    public class QuestionsController : Controller
    {
        private readonly PulseDbContext db;
        private readonly AuthManager authManager;
        private readonly IMapper mapper;
        private readonly IHubContext<StudentHub, IStudentHubClient> studentHub;

        public QuestionsController(
            PulseDbContext db,
            AuthManager authManager,
            IMapper mapper,
            IHubContext<StudentHub, IStudentHubClient> studentHub)
        {
            this.db = db;
            this.authManager = authManager;
            this.mapper = mapper;
            this.studentHub = studentHub;
        }

        [HttpGet]
        public async Task<SessionQuestionDetailsDto[]> List(Guid sessionCheckinId, CancellationToken cancellationToken)
        {
            var questions = await this.db.SessionQuestions
                .Where(m => m.SessionStudent.Session.Class.InstructorId == this.authManager.UserId &&
                            m.SessionCheckinId == sessionCheckinId &&
                            m.Dismissed == null)
                .OrderBy(m => m.Created)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return this.mapper.Map<SessionQuestionDetailsDto[]>(questions);
        }

        [HttpPut("dismiss")]
        public async Task Dismiss(SessionQuestionDismissDto model, CancellationToken cancellationToken)
        {
            var instance = await this.db.SessionQuestions
                .Include(m => m.SessionStudent)
                .FirstOrDefaultAsync(
                    m => m.SessionQuestionId == model.SessionQuestionId &&
                         m.SessionCheckin.Session.Class.InstructorId == this.authManager.UserId, cancellationToken);

            if (instance == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (instance.Dismissed == null)
            {
                instance.Dismissed = DateTimeOffset.Now;
                this.db.SessionQuestions.Update(instance);
                await this.db.SaveChangesAsync(cancellationToken);
            }

            await this.studentHub.Clients.Group(instance.SessionStudent.SessionId.ToString())
                .QuestionDismiss(instance.SessionQuestionId);
        }
    }
}
