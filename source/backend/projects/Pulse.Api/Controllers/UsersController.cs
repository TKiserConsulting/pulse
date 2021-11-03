namespace Pulse.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Common.Extensions;
    using Data;
    using Data.Auth;
    using Extensions;
    using MassTransit;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Users;
    using Persistence;
    using Webfarm.Sdk.Common;
    using Webfarm.Sdk.Common.Extensions;
    using Webfarm.Sdk.Data.ComponentModel;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Web.Api.Data;
    using Webfarm.Sdk.Web.Api.Extensions;

    [GrantTypePrefix(WellKnownApplicationParts.Admin)]
    [Route("[controller]")]
    public class UsersController : BaseCrudController<UserListItemDto, UserDetailsDto, UserCreateDto, UserUpdateDto>
    {
        private readonly PulseDbContext db;
        private readonly IMapper mapper;
        private readonly IExecutionContext executionContext;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(
            PulseDbContext db,
            IMapper mapper,
            IExecutionContext executionContext,
            UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.mapper = mapper;
            this.executionContext = executionContext;
            this.userManager = userManager;
        }

        protected override async Task<(IEnumerable<UserListItemDto>, long?)> FindCore(RangeModel range, FilteringData filterBy, CancellationToken cancellationToken)
        {
            filterBy ??= new FilteringData();
            var q = this.db.Set<ApplicationUser>().AsNoTracking();
            q = this.Filter(q, filterBy);

            var totalRecords = await range.CountTotal(c => q.CountAsync(c), cancellationToken);

            var orderedQuery = this.OrderOnFind(q);

            var windowedQuery = orderedQuery
                .Skip(range.Skip)
                .Take(range.Take);

            var mappedQuery = this.MapOnFind(windowedQuery);

            var data = await mappedQuery
                .ToListAsync(cancellationToken);

            //await this.FillListItems(data, cancellationToken);

            return (data, totalRecords);
        }

        protected virtual IQueryable<ApplicationUser> OrderOnFind(IQueryable<ApplicationUser> query)
        {
            return query.OrderBy(m => m.UserName);
        }

        protected virtual IQueryable<UserListItemDto> MapOnFind(IQueryable<ApplicationUser> query)
        {
            return query
                .ProjectTo<UserListItemDto>(this.mapper.ConfigurationProvider);
        }

        protected virtual IQueryable<ApplicationUser> Filter(IQueryable<ApplicationUser> q, FilteringData filterBy)
        {
            var currentRole = this.executionContext.Principal.Role<UserRole>();

            switch (currentRole)
            {
                case UserRole.Admin:
                    break;
                case UserRole.Instructor:
                case UserRole.Student:
                    throw new UnauthorizedAccessException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentRole), "currentRole");
            }

            if (filterBy.TryGetValue("text", out var text) && !string.IsNullOrEmpty(text))
            {
                q = q.Where(m =>
                    m.UserName.ToLower().Contains(text.ToLower()));
            }

            return q;
        }

        protected override async Task<UserDetailsDto> ReadCore(Guid id, CancellationToken cancellationToken)
        {
            var instance = await this.db.LoadAsync<IdentityUser>(id.ToString(), cancellationToken);
            var result = await this.MapOnRead(instance, cancellationToken);
            return result;
        }

        protected virtual Task<UserDetailsDto> MapOnRead(IdentityUser instance, CancellationToken cancellationToken)
        {
            var mapped = this.mapper.Map<UserDetailsDto>(instance);
            return Task.FromResult(mapped);
        }

        protected override async Task<Guid> CreateCore(UserCreateDto model, CancellationToken cancellationToken)
        {
            var principal = this.executionContext.Principal;
            principal.DemandUserCreate(model.Role);

            var instance = await this.MapOnCreate(model, cancellationToken);

            // create user
            await this.userManager.CreateAsync(instance, model.Password);

            // attach to role
            await this.userManager.AddToRoleAsync(instance, model.Role.ToString());

            return instance.Id;
        }

        protected virtual Task<ApplicationUser> MapOnCreate(UserCreateDto model, CancellationToken cancellationToken)
        {
            var instance = this.mapper.Map<ApplicationUser>(model);
            instance.Id = NewId.NextGuid();
            return Task.FromResult(instance);
        }

        protected override async Task UpdateCore(Guid id, UserUpdateDto model, CancellationToken cancellationToken)
        {
            var instance = await this.userManager.FindByIdAsync(id.ToString());
            instance.DemandUser();

            await this.MapOnUpdate(model, instance, cancellationToken);

            var result = await this.userManager.UpdateAsync(instance);
            result.DemandValid("userName");

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await this.userManager.GeneratePasswordResetTokenAsync(instance);

                var newPasswordResult = await this.userManager.ResetPasswordAsync(instance, token, model.Password);
                newPasswordResult.DemandValid(nameof(model.Password));
            }

            await this.db.SaveChangesAsync(cancellationToken);
        }

        protected virtual Task MapOnUpdate(UserUpdateDto model, ApplicationUser instance, CancellationToken cancellationToken)
        {
            this.mapper.Map(model, instance);
            return Task.CompletedTask;
        }

        protected override async Task DeleteCore(Guid id, CancellationToken cancellationToken)
        {
            var identity = await this.userManager.FindByIdAsync(id.ToString());

            var isSystemAdmin = await this.db.UserClaims.AnyAsync(
                c => c.ClaimType == MetadataInfo.AppClaims.ClaimType && c.ClaimValue == MetadataInfo.AppClaims.Administrator,
                cancellationToken);
            if (isSystemAdmin)
            {
                throw new ApplicationValidationException(
                    new ApplicationValidationResult(string.Empty, errorCode: "SystemAdmin"));
            }

            this.db.Users.Remove(identity);

            await this.db.SaveChangesAsync(cancellationToken);
        }

        protected override Exception MapException(Exception sourceException)
        {
            return sourceException.WrapPersistenceUniqueness(nameof(IdentityUser.UserName));
        }

        //private async Task FillListItems(IList<UserListItemDto> data, CancellationToken cancellationToken)
        //{
        //    foreach (var dto in data)
        //    {
        //        var role = await this.db.UserRoles.SingleAsync(r => r.UserId == dto.Id, cancellationToken);
        //        dto.Role = Enum.Parse<UserRole>(role.RoleId);
        //    }
        //}
    }
}
