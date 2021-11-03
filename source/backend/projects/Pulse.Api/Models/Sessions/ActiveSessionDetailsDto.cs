namespace Pulse.Api.Models.Sessions
{
    using System.ComponentModel.DataAnnotations;

    public class ActiveSessionDetailsDto : SessionDetailsDto
    {
        [Required]
        public SessionEmoticonDetailsDto[] Emoticons { get; set; }

        public SessionCheckinDetailsDto ActiveCheckin { get; set; }
    }
}
