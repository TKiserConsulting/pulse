namespace Pulse.Api.Common.Mappers
{
    using AutoMapper;
    using Data;
    using Models.Classes;

    public class ClassAutoMapperProfile : Profile
    {
        public ClassAutoMapperProfile()
        {
            this.CreateMap<TheClass, ClassDetailsDto>();
            this.CreateMap<TheClass, ClassListItemDto>();
            this.CreateMap<ClassUpsertDto, TheClass>();
        }
    }
}
