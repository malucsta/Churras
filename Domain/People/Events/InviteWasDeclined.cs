using Domain.Common;

namespace Domain.People.Events
{
    public class InviteWasDeclined : IEvent
    {
        public string InviteId { get; set; } = string.Empty;
        public string PersonId { get; set; } = string.Empty;
    }
}
