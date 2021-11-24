namespace Pulse.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Managers;
    using Data;
    using Data.Auth;
    using Hubs;
    using Managers;
    using MassTransit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Models.Authentication;
    using Models.Emoticons;
    using Models.Sessions;
    using Models.Students;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;
    using Webfarm.Sdk.Data.Exceptions;

    [GrantTypePrefix(WellKnownApplicationParts.Student)]
    [Route("[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private const int AccessTokenExpiresMin = 1440;

        private readonly IConfiguration configuration;
        private readonly PulseDbContext db;
        private readonly IMapper mapper;
        private readonly AuthManager authManager;
        private readonly SessionManager sessionManager;
        private readonly IHubContext<InstructorHub, IInstructorHubClient> instructorHub;

        public StudentsController(
            IConfiguration configuration,
            PulseDbContext db,
            IMapper mapper,
            AuthManager authManager,
            SessionManager sessionManager,
            IHubContext<InstructorHub, IInstructorHubClient> instructorHub)
        {
            this.configuration = configuration;
            this.db = db;
            this.mapper = mapper;
            this.authManager = authManager;
            this.sessionManager = sessionManager;
            this.instructorHub = instructorHub;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<JoinSessionResultDto> Signin(JoinSessionRequestDto model, CancellationToken cancellationToken)
        {
            var session =
                await this.db.Sessions
                    .Where(m => m.Code == model.SessionCode && m.Finished == null)
                    .Include(m => m.Class)
                    .FirstOrDefaultAsync(cancellationToken);

            if (session == null ||
                await this.sessionManager.FinishExpiredSession(session, session.Class.InstructorId,
                    cancellationToken) == null)
            {
                throw new UnauthenticatedException($"There is no active session for code '{model.SessionCode}'");
            }

            // create new session student
            var sessionStudentId = NewId.NextGuid();
            await this.db.SessionStudents.AddAsync(new SessionStudent
            {
                Id = sessionStudentId,
                SessionId = session.Id,
                CreatedBy = sessionStudentId.ToString()
            }, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);

            // generate tokens
            var accessToken = this.GenerateAccessToken(sessionStudentId);
            var refreshToken = this.GenerateRefreshToken();

            var signinResult = new JoinSessionResultDto
            {
                TokenInfo = new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                },
                SessionId = session.Id,
                SessionStudentId = sessionStudentId
            };

            await this.instructorHub.Clients.Group(session.Id.ToString()).StudentJoin(sessionStudentId);

            return signinResult;
        }

        [HttpGet("session")]
        public async Task<StudentSessionDetailsDto> GetSession([FromQuery] GetSessionRequestDto request, CancellationToken cancellationToken)
        {
            var sessionId = request.SessionId;

            if (sessionId == Guid.Empty)
            {
                sessionId = await this.db.SessionStudents
                    .Where(m => m.SessionStudentId == this.authManager.UserId)
                    .Select(m => m.SessionId)
                    .SingleOrDefaultAsync(cancellationToken);
            }

            var instance = await this.db.Sessions
                .Include(m => m.Class)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                m => m.SessionId == sessionId, cancellationToken);

            if (instance == null || instance.Finished.HasValue)
            {
                return null;
            }

            var model = this.mapper.Map<StudentSessionDetailsDto>(instance);

            var emoticons = await this.db.InstructorEmoticons
                .Where(m => m.InstructorId == instance.Class.InstructorId)
                .OrderBy(m => m.SortIndex)
                .ThenBy(m => m.Created)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            model.Emoticons = this.mapper.Map<InstructorEmoticonListItemDto[]>(emoticons);

            var checkin =
                await this.db.SessionCheckins
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.SessionId == sessionId && m.Finished == null, cancellationToken);

            model.ActiveCheckin = this.mapper.Map<SessionCheckinDetailsDto>(checkin);

            var questions = await this.db.SessionQuestions
                .Where(m => m.SessionStudentId == this.authManager.UserId && m.Dismissed == null)
                .OrderBy(m => m.Created)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            model.Questions = this.mapper.Map<SessionQuestionDetailsDto[]>(questions);

            var instructorSettings = await this.db.InstructorSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.InstructorId == instance.Class.InstructorId, cancellationToken);

            model.EmoticonTapDelaySeconds = instructorSettings?.EmoticonTapDelaySeconds ?? InstructorSettings.DefaultEmoticonTapDelaySeconds;

            return model;
        }

        [HttpPost("tap")]
        public async Task Tap(EmoticonTapDto model, CancellationToken cancellationToken)
        {
            var student =
                await this.db.SessionStudents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.SessionStudentId == this.authManager.UserId, cancellationToken);

            if (student == null)
            {
                throw new UnauthorizedAccessException();
            }

            var session = await this.db.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.SessionId == student.SessionId, cancellationToken);

            if (session == null || session.Finished.HasValue)
            {
                throw new UnauthorizedAccessException();
            }

            await this.db.EmoticonTaps.AddAsync(new EmoticonTap
            {
                EmoticonTapId = NewId.NextGuid(),
                SessionStudentId = student.SessionStudentId,
                InstructorEmoticonId = model.InstructorEmoticonId,
                TimeTicks = DateTime.Now.Ticks
            }, cancellationToken);

            await this.db.SaveChangesAsync(cancellationToken);

            await this.instructorHub.Clients.Group(session.SessionId.ToString()).EmoticonTap(model.InstructorEmoticonId);
        }

        [HttpPost("question")]
        public async Task<Guid?> CreateQuestion(SessionQuestionCreateDto model, CancellationToken cancellationToken)
        {
            var checkin = await this.db.SessionCheckins
                .Include(m => m.Session)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    m => m.SessionId == model.SessionId && m.Finished == null, cancellationToken);

            if (checkin == null)
            {
                return null;
            }

            if (checkin.Session.Finished.HasValue)
            {
                throw new UnauthorizedAccessException("The session has ended");
            }

            var instance = new SessionQuestion
            {
                SessionQuestionId = NewId.NextGuid(),
                SessionCheckinId = checkin.SessionCheckinId,
                SessionStudentId = this.authManager.UserId,
                Text = model.Text,
                Created = DateTimeOffset.Now
            };

            await this.db.SessionQuestions.AddAsync(instance, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);

            // notify instructor
            var questionDto = this.mapper.Map<SessionQuestionDetailsDto>(instance);
            await this.instructorHub.Clients.Group(checkin.SessionId.ToString()).Question(questionDto);

            return instance.SessionQuestionId;
        }

        /*
        [HttpPost]
        [Route("signout")]
        public async Task Signout(CancellationToken cancellationToken)
        {
            var principal = this.User;

            if (principal != null && (principal.Identity?.IsAuthenticated ?? false))
            {
                // remove all refresh tokens
                var identityUserId = Guid.Parse(principal.UserId());
                await this.RemoveRefreshTokens(identityUserId, cancellationToken);

                // signout
                await this.signInManager.SignOutAsync();
            }
        }
        */

        private string GenerateAccessToken(Guid sessionStudentId)
        {
            var expiresMinutes = this.configuration.GetValue<int?>("StudentTokens:Expires");
            var encryptionKey = this.configuration.GetValue<string>("Tokens:Key");
            var securityKey = Encoding.UTF8.GetBytes(encryptionKey);
            var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, sessionStudentId.ToString()),
                new Claim(ClaimTypes.Name, sessionStudentId.ToString()),
                new Claim(ClaimTypes.Role, nameof(UserRole.Student))
            };

            var token = new JwtSecurityToken(
                issuer: this.configuration.GetValue<string>("StudentTokens:Issuer"),
                audience: this.configuration.GetValue<string>("StudentTokens:Audience"),
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes ?? AccessTokenExpiresMin),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
