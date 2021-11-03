namespace Pulse.Api.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Managers;
    using Data.Auth;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Emoticons;
    using Persistence;
    using Webfarm.Sdk.Data.ComponentModel;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    public class EmoticonsController : Controller
    {
        private readonly PulseDbContext db;
        private readonly AuthManager authManager;
        private readonly IMapper mapper;

        public EmoticonsController(
            PulseDbContext db,
            AuthManager authManager,
            IMapper mapper)
        {
            this.db = db;
            this.authManager = authManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<InstructorEmoticonListItemDto[]> List(CancellationToken cancellationToken)
        {
            var emoticons = await this.db.InstructorEmoticons
                .Where(m => m.InstructorId == this.authManager.UserId)
                .OrderBy(m => m.SortIndex)
                .ThenBy(m => m.Created)
                .ToListAsync(cancellationToken);

            return this.mapper.Map<InstructorEmoticonListItemDto[]>(emoticons);
        }

        [HttpPut]
        public async Task Update(InstructorEmoticonsUpdateDto model, CancellationToken cancellationToken)
        {
            var emoticons = await this.db.InstructorEmoticons
                .Where(m => m.InstructorId == this.authManager.UserId)
                .ToListAsync(cancellationToken);

            if (emoticons.Count == 0)
            {
                throw new UnauthorizedAccessException();
            }

            foreach (var item in model.Items)
            {
                var emoticon = emoticons.FirstOrDefault(m => m.InstructorEmoticonId == item.Id);
                if (emoticon != null)
                {
                    emoticon.Title = item.Title;
                    emoticon.Color = item.Color;
                    this.db.InstructorEmoticons.Update(emoticon);
                }
            }

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
