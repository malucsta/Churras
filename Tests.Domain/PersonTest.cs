using Domain.People;
using Domain.People.Events;

namespace Tests.Domain
{
    public class PersonTest
    {
        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldChangeFields_When_PersonHasBeenCreated()
        {
            var person = new Person();
            var name = "Maria Luísa";
            var id = Guid.NewGuid().ToString();

            var @event = new PersonHasBeenCreated(id, name, false);
            var result = person.Apply(@event);

            Assert.True(result.IsSuccess);
            Assert.Equal(id.ToString(), person.Id);
            Assert.Equal(name, person.Name);
            Assert.False(person.IsCoOwner);
        }

        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldChangeFields_When_PersonHasBeenInvitedToBbq()
        {
            var person = new Person();
            var reason = "Nova integrante da equipe!";
            var id = Guid.NewGuid().ToString();
            var date = DateTime.UtcNow.AddDays(1);

            var @event = new PersonHasBeenInvitedToBbq(id, date, reason);
            var result = person.Apply(@event);

            Assert.True(result.IsSuccess);
            Assert.Single(person.Invites.Where(x => x.Id == id));
        }

        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldNotAddInviteAgain_When_PersonHasAlreadyBeenInvitedToBbq()
        {
            var person = new Person();
            var reason = "Nova integrante da equipe!";
            var id = Guid.NewGuid().ToString();
            var date = DateTime.UtcNow.AddDays(1);

            var @event = new PersonHasBeenInvitedToBbq(id, date, reason);
            person.Apply(@event);
            person.Apply(@event);

            Assert.Single(person.Invites.Where(x => x.Id == id));
        }

        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldChangeStatus_When_InviteWasAccepted()
        {
            var person = new Person();
            var id = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();
            var date = DateTime.UtcNow.AddDays(2);
            var reason = "Nova integrante da equipe!";

            var @inviteEvent = new PersonHasBeenInvitedToBbq(id, date, reason);
            person.Apply(@inviteEvent);
            var @event = new InviteWasAccepted { InviteId = id, IsVeg = true, PersonId = personId };
            var result = person.Apply(@event);

            Assert.True(result.IsSuccess);
            Assert.Equal(InviteStatus.Accepted, person.Invites.First(x => x.Id == id).Status);
        }

        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldReturnFail_When_InviteWasAccepted_InviteNotFound()
        {
            var person = new Person();
            var id = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();
            
            var @event = new InviteWasAccepted { InviteId = id, IsVeg = true, PersonId = personId };
            var result = person.Apply(@event);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldChangeStatus_When_InviteWasDeclined()
        {
            var person = new Person();
            var id = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();
            var date = DateTime.UtcNow.AddDays(2);
            var reason = "Nova integrante da equipe!";

            var @inviteEvent = new PersonHasBeenInvitedToBbq(id, date, reason);
            person.Apply(@inviteEvent);
            var @event = new InviteWasDeclined { InviteId = id, PersonId = personId };
            var result = person.Apply(@event);

            Assert.True(result.IsSuccess);
            Assert.Equal(InviteStatus.Declined, person.Invites.First(x => x.Id == id).Status);
        }

        [Fact]
        [Trait("Entity", "Person")]
        public void ShouldReturnFail_When_InviteWasDeclined_InviteNotFound()
        {
            var person = new Person();
            var id = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();

            var @event = new InviteWasDeclined { InviteId = id, PersonId = personId };
            var result = person.Apply(@event);

            Assert.False(result.IsSuccess);
        }
    }
}
