namespace Pulse.Api.Common
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Webfarm.Sdk.Data;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;
    using Webfarm.Sdk.Web.Api.Filters;

    [Route("[controller]")]
    public abstract class BaseCrudController<TListItemDto, TDetailsDto, TCreateDto, TUpdateDto> : Controller
        where TListItemDto : class, IIdentified<Guid>
        where TDetailsDto : class, IIdentified<Guid>
    {
        [HttpGet]
        [Route("")]
        public virtual async Task<IEnumerable<TListItemDto>> Find(
            RangeModel range,
            FilteringData filterBy = null,
            CancellationToken cancellationToken = default)
        {
            var (data, totalRecords) = await this.FindCore(range, filterBy, cancellationToken);
            return this.Paged(data, range, totalRecords);
        }

        [HttpGet("{id:guid}")]
        public virtual async Task<TDetailsDto> Read(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var model = await this.ReadCore(id, cancellationToken);
            return model;
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public virtual async Task<ActionResult<TDetailsDto>> Create(
            [BindRequired] TCreateDto model,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.CreateInternal(model, cancellationToken);

                this.Response.StatusCode = (int)HttpStatusCode.Created;
                const string actionName = nameof(this.Read);
                return this.CreatedAtAction(
                    actionName,
                    new { version = this.HttpContext?.GetRequestedApiVersion()?.ToString(), result.Id },
                    result);
            }
            catch (Exception ex)
            {
                if (this.TryMapException(ex, out var mapped))
                {
                    throw mapped;
                }

                throw;
            }
        }

        [HttpPut("{id:guid}")]
        public virtual async Task<TDetailsDto> Update(
            Guid id,
            [BindRequired] TUpdateDto model,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.UpdateInternal(id, model, cancellationToken);
            }
            catch (Exception ex)
            {
                if (this.TryMapException(ex, out var mapped))
                {
                    throw mapped;
                }

                throw;
            }
        }

        [HttpDelete("{id:guid}")]
        [ResponseCode(HttpStatusCode.NoContent)]
        public virtual async Task Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await this.DeleteCore(id, cancellationToken);
            }
            catch (Exception ex)
            {
                if (this.TryMapException(ex, out var mapped))
                {
                    throw mapped;
                }

                throw;
            }
        }

        protected async Task<TDetailsDto> CreateInternal(TCreateDto model, CancellationToken cancellationToken)
        {
            var id = await this.CreateCore(model, cancellationToken);
            var result = await this.ReadCore(id, cancellationToken);
            return result;
        }

        protected async Task<TDetailsDto> UpdateInternal(Guid id, TUpdateDto model, CancellationToken cancellationToken)
        {
            await this.UpdateCore(id, model, cancellationToken);
            var result = await this.ReadCore(id, cancellationToken);
            return result;
        }

        protected abstract Task<(IEnumerable<TListItemDto>, long?)> FindCore(
            RangeModel range,
            FilteringData filterBy,
            CancellationToken cancellationToken);

        protected abstract Task<TDetailsDto> ReadCore(Guid id, CancellationToken cancellationToken);

        protected abstract Task<Guid> CreateCore(TCreateDto model, CancellationToken cancellationToken);

        protected abstract Task UpdateCore(Guid id, TUpdateDto model, CancellationToken cancellationToken);

        protected abstract Task DeleteCore(Guid id, CancellationToken cancellationToken);

        protected bool TryMapException(Exception sourceException, out Exception mappedException)
        {
            mappedException = this.MapException(sourceException);
            return mappedException != null;
        }

        protected virtual Exception MapException(Exception sourceException)
        {
            return null;
        }
    }
}
