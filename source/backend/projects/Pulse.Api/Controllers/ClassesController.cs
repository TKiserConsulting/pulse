namespace Pulse.Api.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Data;
    using Data.Auth;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Classes;
    using Persistence;
    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data.ComponentModel;
    using Webfarm.Sdk.Web.Api.Data;

    [GrantTypePrefix(WellKnownApplicationParts.Instructor)]
    [Route("[controller]")]
    public class ClassesController : BaseDbCrudController<TheClass, ClassListItemDto, ClassDetailsDto, ClassUpsertDto, ClassUpsertDto>
    {
        private readonly IExecutionContext executionContext;

        public ClassesController(PulseDbContext db, IMapper mapper, IExecutionContext executionContext)
            : base(db, mapper)
        {
            this.executionContext = executionContext;
        }

        protected override IQueryable<TheClass> OrderOnFind(IQueryable<TheClass> query)
        {
            return query.OrderBy(m => m.Name);
        }

        protected override IQueryable<TheClass> Filter(IQueryable<TheClass> q, FilteringData filterBy)
        {
            q = q.Where(m => m.InstructorId == Guid.Parse(this.executionContext.UserId()));

            return q;
        }

        protected override async Task<TheClass> MapOnCreate(ClassUpsertDto model, CancellationToken cancellationToken)
        {
            var instance = await base.MapOnCreate(model, cancellationToken);
            instance.InstructorId = Guid.Parse(this.executionContext.UserId());
            return instance;
        } 

        protected override Exception MapException(Exception sourceException)
        {
            return sourceException.WrapPersistenceUniqueness(nameof(TheClass.Name));
        }
    }
}
