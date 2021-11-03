namespace Pulse.Api.Common.Mappers
{
    using AutoMapper;
    using Data;
    using Models.Sessions;

    public class SessionAutoMapperProfile : Profile
    {
        public SessionAutoMapperProfile()
        {
            this.CreateMap<Session, SessionDetailsDto>();
            this.CreateMap<Session, ActiveSessionDetailsDto>();
            this.CreateMap<Session, SessionListItemDto>();
            this.CreateMap<SessionCreateDto, Session>();

            this.CreateMap<SessionCheckin, SessionCheckinDetailsDto>();
            this.CreateMap<SessionCheckinCreateDto, SessionCheckin>();

            this.CreateMap<SessionQuestion, SessionQuestionDetailsDto>();
            this.CreateMap<SessionQuestionCreateDto, SessionQuestion>();
        }
    }
}
