namespace Pulse.Api.Hubs
{
    using System;
    using System.Threading.Tasks;
    using Models.Sessions;

    public interface IStudentHubClient
    {
        Task Checkin(SessionCheckinDetailsDto checkinDetails);

        Task CheckinFinish(Guid sessionCheckinId);

        Task QuestionDismiss(Guid sessionQuestionId);

        Task SessionFinish(Guid sessionId);
    }
}
