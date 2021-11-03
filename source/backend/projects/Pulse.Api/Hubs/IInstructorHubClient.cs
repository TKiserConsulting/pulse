namespace Pulse.Api.Hubs
{
    using System;
    using System.Threading.Tasks;
    using Models.Sessions;

    public interface IInstructorHubClient
    {
        Task StudentJoin(Guid sessionStudentId);

        Task EmoticonTap(Guid instructorEmoticonId);

        Task Question(SessionQuestionDetailsDto question);

        Task SessionFinish();
    }
}
