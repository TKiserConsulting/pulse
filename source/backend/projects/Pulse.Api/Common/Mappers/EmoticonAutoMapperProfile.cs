namespace Pulse.Api.Common.Mappers
{
    using AutoMapper;
    using Data;
    using Models.Emoticons;
    using Models.Sessions;

    public class EmoticonAutoMapperProfile : Profile
    {
        public EmoticonAutoMapperProfile()
        {
            this.CreateMap<InstructorEmoticon, InstructorEmoticonDetailsDto>();
            this.CreateMap<InstructorEmoticon, InstructorEmoticonListItemDto>();
            this.CreateMap<InstructorEmoticonUpdateDto, InstructorEmoticon>();

            this.CreateMap<InstructorEmoticon, SessionEmoticonDetailsDto>();

            this.CreateMap<Emoticon, InstructorEmoticon>()
                .ForMember(m => m.Id, opt => opt.Ignore());
        }
    }
}
