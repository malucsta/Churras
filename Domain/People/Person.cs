using Domain.Common;
using Domain.People.Errors;
using Domain.People.Events;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.People
{
    public class Person : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCoOwner { get; set; }
        public IEnumerable<Invite> Invites { get; set; }

        public Person()
        {
            Invites = new List<Invite>();
        }

        public Result When(PersonHasBeenCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;
            IsCoOwner = @event.IsCoOwner;

            return Result.Ok();
        }

        public Result When(PersonHasBeenInvitedToBbq @event)
        {
            Invites = Invites.Append(new Invite
            {
                Id = @event.Id,
                Date = @event.Date,
                Bbq = $"{@event.Date} - {@event.Reason}",
                Status = InviteStatus.Pending
            });

            return Result.Ok();
        }

        public Result When(InviteWasAccepted @event)
        {
            var invite = Invites.FirstOrDefault(x => x.Id == @event.InviteId);
            
            if (invite is null)
                return Result.Fail(new InviteNotFoundError(@event.InviteId));
            
            invite.Status = InviteStatus.Accepted;
            return Result.Ok();
        }

        public Result When(InviteWasDeclined @event)
        {
            var invite = Invites.FirstOrDefault(x => x.Id == @event.InviteId);
            
            if (invite is null)
                return Result.Fail(new InviteNotFoundError(@event.InviteId));

            invite.Status = InviteStatus.Declined;
            return Result.Ok();
        }

        public object? TakeSnapshot()
        {
            return new
            {
                Id,
                Name,
                IsCoOwner,
                Invites = Invites.Where(o => o.Status != InviteStatus.Declined)
                                .Where(o => o.Date > DateTime.Now)
                                .Select(o => new { o.Id, o.Bbq, Status = o.Status.ToString() })
            };
        }
    }
}
