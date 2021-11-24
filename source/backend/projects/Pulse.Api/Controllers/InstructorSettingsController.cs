namespace Pulse.Api.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common.Managers;
    using Data;
    using Data.Auth;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;
    using Models.Settings;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    [ApiController]
    public class InstructorSettingsController : ControllerBase
    {
        private readonly PulseDbContext db;
        private readonly IMapper mapper;
        private readonly AuthManager authManager;

        public InstructorSettingsController(
            PulseDbContext db,
            IMapper mapper,
            AuthManager authManager)
        {
            this.db = db;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpGet]
        public async Task<InstructorSettingsDetailsDto> Load(CancellationToken cancellationToken)
        {
            var dto = await this.db.InstructorSettings
                .Where(m => m.InstructorId == this.authManager.UserId)
                .AsNoTracking()
                .ProjectTo<InstructorSettingsDetailsDto>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            dto ??= new InstructorSettingsDetailsDto
            {
                InstructorId = this.authManager.UserId,
                SessionTimeoutHours = InstructorSettings.DefaultSessionTimeoutHours,
                EmoticonTapDelaySeconds = InstructorSettings.DefaultEmoticonTapDelaySeconds
            };

            return dto;
        }

        [HttpPost]
        public async Task Update(InstructorSettingsUpdateDto model, CancellationToken cancellationToken)
        {
            var instance = await this.db.InstructorSettings
                .FirstOrDefaultAsync(m => m.InstructorId == this.authManager.UserId, cancellationToken);

            if (instance == null)
            {
                instance = this.mapper.Map<InstructorSettings>(model);
                instance.InstructorId = this.authManager.UserId;
                instance.CreatedBy = this.authManager.UserId.ToString();
                instance.Created = DateTimeOffset.Now;
                await this.db.AddAsync(instance, cancellationToken);
            }
            else
            {
                this.mapper.Map(model, instance);
                instance.ModifiedBy = this.authManager.UserId.ToString();
                instance.Modified = DateTimeOffset.Now;
                this.db.Entry(instance).State = EntityState.Modified;
            }

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
