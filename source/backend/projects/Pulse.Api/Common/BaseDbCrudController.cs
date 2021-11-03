namespace Pulse.Api.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using Webfarm.Sdk.Data;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    public abstract class BaseDbCrudController<TEntity, TListItemDto, TDetailsDto, TCreateDto, TUpdateDto>
        : BaseCrudController<TListItemDto, TDetailsDto, TCreateDto, TUpdateDto>
        where TEntity : class, IIdentified<Guid>, new()
        where TListItemDto : class, IIdentified<Guid>
        where TDetailsDto : class, IIdentified<Guid>
    {
        protected BaseDbCrudController(PulseDbContext db, IMapper mapper)
        {
            this.Db = db;
            this.Mapper = mapper;
        }

        protected PulseDbContext Db { get; }

        protected IMapper Mapper { get; }

        protected override async Task<(IEnumerable<TListItemDto>, long?)> FindCore(RangeModel range, FilteringData filterBy, CancellationToken cancellationToken)
        {
            filterBy ??= new FilteringData();
            var q = this.Db.Set<TEntity>().AsNoTracking();
            q = this.Filter(q, filterBy);

            var totalRecords = await range.CountTotal(c => q.CountAsync(c), cancellationToken);

            var orderedQuery = this.OrderOnFind(q);

            var windowedQuery = orderedQuery
                .Skip(range.Skip)
                .Take(range.Take);

            var mappedQuery = this.MapOnFind(windowedQuery);

            var data = await mappedQuery
                .ToListAsync(cancellationToken);

            return (data, totalRecords);
        }

        protected virtual IQueryable<TEntity> OrderOnFind(IQueryable<TEntity> query)
        {
            return query; // .OrderBy(m => m.Id);
        }

        protected virtual IQueryable<TListItemDto> MapOnFind(IQueryable<TEntity> query)
        {
            return query
                .ProjectTo<TListItemDto>(this.Mapper.ConfigurationProvider);
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> q, FilteringData filterBy)
        {
            return q;
        }

        protected override async Task<TDetailsDto> ReadCore(Guid id, CancellationToken cancellationToken)
        {
            var instance = await this.Db.LoadAsync<TEntity>(id, cancellationToken);
            var result = await this.MapOnRead(instance, cancellationToken);
            return result;
        }

        protected virtual Task<TDetailsDto> MapOnRead(TEntity instance, CancellationToken cancellationToken)
        {
            var mapped = this.Mapper.Map<TDetailsDto>(instance);
            return Task.FromResult(mapped);
        }

        protected override async Task<Guid> CreateCore(TCreateDto model, CancellationToken cancellationToken)
        {
            var instance = await this.MapOnCreate(model, cancellationToken);
            await this.Db.Set<TEntity>().AddAsync(instance, cancellationToken);
            await this.Db.SaveChangesAsync(cancellationToken);
            return instance.Id;
        }

        protected virtual Task<TEntity> MapOnCreate(TCreateDto model, CancellationToken cancellationToken)
        {
            var instance = this.Mapper.Map<TEntity>(model);
            instance.Id = NewId.NextGuid();
            return Task.FromResult(instance);
        }

        protected override async Task UpdateCore(Guid id, TUpdateDto model, CancellationToken cancellationToken)
        {
            var instance = await this.Db.LoadAsync<TEntity>(id, cancellationToken);
            await this.MapOnUpdate(model, instance, cancellationToken);
            await this.Db.SaveChangesAsync(cancellationToken);
        }

        protected virtual Task MapOnUpdate(TUpdateDto model, TEntity instance, CancellationToken cancellationToken)
        {
            this.Mapper.Map(model, instance);
            return Task.CompletedTask;
        }

        protected override async Task DeleteCore(Guid id, CancellationToken cancellationToken)
        {
            await this.Db.RemoveAsync<TEntity>(id, cancellationToken);
        }
    }
}
