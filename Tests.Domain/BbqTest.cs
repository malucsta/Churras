using Azure.Core;
using Domain.Bbqs;
using Domain.Bbqs.Events;
using Domain.People;
using Domain.People.Events;
using System.Collections.Generic;

namespace Tests.Domain
{
    public class BbqTest
    {
        [Fact]
        [Trait("Entity", "Bbq")]
        public void ShouldChangeFields_When_ThereIsSomeoneElseInTheMood()
        {
            var bbq = new Bbq();
            var date = DateTime.Now;
            var reason = "Nova integrante da equipe!";
            var id = Guid.NewGuid();

            var @event = new ThereIsSomeoneElseInTheMood(id, date, reason, false);
            var result = bbq.Apply(@event);

            Assert.True(result.IsSuccess);
            Assert.Equal(id.ToString(), bbq.Id);
            Assert.Equal(date, bbq.Date);
            Assert.Equal(reason, bbq.Reason);
            Assert.Equal(BbqStatus.New, bbq.Status);
        }

        [Theory]
        [Trait("Entity", "Bbq")]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ShouldChangeFields_When_BbqStatusUpdated(bool gonnaHappen, bool trincaWillPay)
        {
            var bbq = new Bbq();

            var @event = new BbqStatusUpdated(gonnaHappen, trincaWillPay);
            var result = bbq.Apply(@event);

            Assert.True(result.IsSuccess);
            
            if(gonnaHappen)
                Assert.Equal(BbqStatus.PendingConfirmations, bbq.Status);
            else
                Assert.Equal(BbqStatus.ItsNotGonnaHappen, bbq.Status);

            if(trincaWillPay)
                Assert.True(bbq.IsTrincasPaying);
            else
                Assert.False(bbq.IsTrincasPaying);
        }

        [Theory]
        [Trait("Entity", "Bbq")]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldChangeFields_When_InviteWasAccepted(bool isVeg)
        {
            var bbq = new Bbq();
            var inviteId = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();

            var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = isVeg, PersonId = personId };
            var result = bbq.Apply(@event);

            var list = bbq.ShoppingList[personId];

            Assert.True(result.IsSuccess);
            Assert.True(bbq.ShoppingList.ContainsKey(personId));
            Assert.True(bbq.NumberOfConfirmations == 1);
            
            if(isVeg)
            {
                Assert.Equal(0.6m, list.VegetablesInKilogram);
                Assert.Equal(0, list.MeatInKilogram);
            }
            else
            {
                Assert.Equal(0.3m, list.VegetablesInKilogram);
                Assert.Equal(0.3m, list.MeatInKilogram);
            } 
        }

        [Fact]
        [Trait("Entity", "Bbq")]
        public void ShouldReturn_When_InviteWasAlreadyAccepted()
        {
            var bbq = new Bbq();
            var inviteId = Guid.NewGuid().ToString();
            bool isVeg = true;
            var personId = Guid.NewGuid().ToString();

            var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = isVeg, PersonId = personId };
            var result = bbq.Apply(@event);

            var secondResult = bbq.Apply(@event);

            var list = bbq.ShoppingList[personId];
            
            Assert.True(result.IsSuccess);
            Assert.Single(bbq.ShoppingList.Keys.Where(x => x == personId));
            Assert.True(bbq.NumberOfConfirmations == 1);
            Assert.Equal(0.6m, list.VegetablesInKilogram);
            Assert.Equal(0, list.MeatInKilogram);
        }

        [Fact]
        [Trait("Entity", "Bbq")]
        public void ShouldBeConfirmed_When_SevenInvitesAreAccepted()
        {
            var bbq = new Bbq();
            var inviteId = Guid.NewGuid().ToString();
            
            var personId = Guid.NewGuid().ToString();
            var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            personId = Guid.NewGuid().ToString();
            @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            personId = Guid.NewGuid().ToString();
            @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            personId = Guid.NewGuid().ToString();
            @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            personId = Guid.NewGuid().ToString();
            @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            personId = Guid.NewGuid().ToString();
            @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            personId = Guid.NewGuid().ToString();
            @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = true, PersonId = personId };
            bbq.Apply(@event);

            Assert.True(bbq.NumberOfConfirmations == 7);
            Assert.Equal(BbqStatus.Confirmed, bbq.Status);
        }

        [Fact]
        [Trait("Entity", "Bbq")]
        public void ShouldChangeFields_When_InviteAlreadyAcceptedWasDeclined()
        {
            var bbq = new Bbq();
            var inviteId = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();

            var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = false, PersonId = personId };
            bbq.Apply(@event);

            var @newEvent = new InviteWasDeclined { InviteId = inviteId, PersonId = personId };
            var result = bbq.Apply(@newEvent);

            Assert.True(result.IsSuccess);
            Assert.False(bbq.ShoppingList.ContainsKey(personId));
            Assert.True(bbq.NumberOfConfirmations == 0);
        }

        [Fact]
        [Trait("Entity", "Bbq")]
        public void ShouldReturnFail_When_InviteAlreadyAcceptedWasDeclined_InviteWasNotFound()
        {
            var bbq = new Bbq();
            var inviteId = Guid.NewGuid().ToString();
            var personId = Guid.NewGuid().ToString();

            var @newEvent = new InviteWasDeclined { InviteId = inviteId, PersonId = personId };
            var result = bbq.Apply(@newEvent);

            Assert.True(result.IsFailed);
        }
    }
}