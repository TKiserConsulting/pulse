namespace Pulse.Api.Common.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AutoMapper;
    using Persistence;
    using Webfarm.Sdk.Data;

    public static class AutoMapperProfileExtensions
    {
        public static void Preload<TEntity, TComponent>(
            this ResolutionContext context,
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TComponent>>> getterExpression)
            where TEntity : class, IIdentified<Guid>
            where TComponent : class
        {
            var db = (PulseDbContext)context.Options.ServiceCtor(typeof(PulseDbContext));
            if (entity.Id != Guid.Empty)
            {
                db.Entry(entity).Collection(getterExpression).Load();
            }
        }

        public static void Preload<TEntity, TComponent>(
            this ResolutionContext context,
            TEntity entity,
            Expression<Func<TEntity, TComponent>> getterExpression)
            where TEntity : class, IIdentified<Guid>
            where TComponent : class
        {
            var db = (PulseDbContext)context.Options.ServiceCtor(typeof(PulseDbContext));
            if (entity.Id != Guid.Empty)
            {
                db.Entry(entity).Reference(getterExpression).Load();
            }
        }
    }
}
