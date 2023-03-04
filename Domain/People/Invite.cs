using System;

namespace Domain.People
{
    public class Invite
    {
        public string Id { get; set; } = string.Empty;
        public string Bbq { get; set; } = string.Empty;
        public InviteStatus Status { get; set; }
        public DateTime Date { get; set; }
    }

    public enum InviteStatus
    {
        Pending,
        Accepted,
        Declined
    }
}
