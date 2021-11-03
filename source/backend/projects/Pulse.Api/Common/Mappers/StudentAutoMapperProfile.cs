namespace Pulse.Api.Common.Mappers
{
    using AutoMapper;
    using Data;
    using Models.Students;

    public class StudentAutoMapperProfile : Profile
    {
        public StudentAutoMapperProfile()
        {
            this.CreateMap<Session, StudentSessionDetailsDto>();
            this.CreateMap<SessionStudent, SessionStudentDetailsDto>();
        }
    }
}
