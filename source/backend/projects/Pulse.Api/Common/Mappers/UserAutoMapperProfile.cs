namespace Pulse.Api.Common.Mappers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Data.Auth;
    using Models.Users;
    using Persistence;

    public class UserAutoMapperProfile : Profile
    {
        public UserAutoMapperProfile()
        {
            this.CreateMap<ApplicationUser, UserListItemDto>();

            this.CreateMap<ApplicationUser, UserDetailsDto>()
                .AfterMap(AfterMapConfigure)
                ;

            this.CreateMap<UserCreateDto, ApplicationUser>()
                .ForMember(m => m.LockoutEnabled, opts => opts.MapFrom(i => false))
                .ForMember(m => m.EmailConfirmed, opts => opts.MapFrom(i => true))
                ;
            this.CreateMap<UserUpdateDto, ApplicationUser>();

            this.CreateMap<RegisterRequestDto, ApplicationUser>()
                .ForMember(m => m.LockoutEnabled, opts => opts.MapFrom(i => false))
                .ForMember(m => m.EmailConfirmed, opts => opts.MapFrom(i => true))
                ;
        }

        private static void AfterMapConfigure(ApplicationUser instance, UserDetailsDto model, ResolutionContext context)
        {
            var db = (PulseDbContext)context.Options.ServiceCtor(typeof(PulseDbContext));
            var userRole = db.UserRoles.Single(r => r.UserId == instance.Id);
            var role = db.Roles.Single(r => r.Id == userRole.RoleId);
            model.Role = Enum.Parse<UserRole>(role.Name);
        }
    }
}
