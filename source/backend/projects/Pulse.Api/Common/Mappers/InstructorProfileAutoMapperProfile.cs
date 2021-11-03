namespace Pulse.Api.Common.Mappers
{
    using AutoMapper;
    using Data;
    using Models.Profiles;

    public class InstructorProfileAutoMapperProfile : Profile
    {
        public InstructorProfileAutoMapperProfile()
        {
            this.CreateMap<InstructorProfile, InstructorProfileDetailsDto>();
            this.CreateMap<InstructorProfileUpdateDto, InstructorProfile>();
        }
    }
}
