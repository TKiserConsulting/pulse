namespace Pulse.Api.Common.Mappers
{
    using AutoMapper;
    using Data;
    using Models.Settings;

    public class InstructorSettingsAutoMapperProfile : Profile
    {
        public InstructorSettingsAutoMapperProfile()
        {
            this.CreateMap<InstructorSettings, InstructorSettingsDetailsDto>();
            this.CreateMap<InstructorSettingsUpdateDto, InstructorSettings>();
        }
    }
}
