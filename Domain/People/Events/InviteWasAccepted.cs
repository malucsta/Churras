using Domain.Common;

namespace Domain.People.Events
{
    public class InviteWasAccepted : IEvent
    {
        public string PersonId { get; set; } = string.Empty;
        public string InviteId { get; set; } = string.Empty;
        public bool IsVeg { get; set; }
    }
}
