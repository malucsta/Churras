using CrossCutting;
using Domain.Bbqs.Errors;
using Domain.Bbqs.Events;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.Lookups;
using Domain.People.Errors;
using Domain.People.Events;
using Domain.People.Repositories;
using FluentResults;

namespace Application.UseCases.Bbqs
{
    public class ModerateBbq : IModerateBbq
    {
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _persons;
        private readonly IBbqRepository _repository;

        public ModerateBbq(IBbqRepository repository, SnapshotStore snapshots, IPersonRepository persons)
        {
            _persons = persons;
            _snapshots = snapshots;
            _repository = repository;
        }
        public async Task<Result<object>> Execute(ModerateBbqRequest request)
        {
            var bbq = await _repository.GetAsync(request.Id);
            
            if (bbq is null)
                return Result.Fail(new BbqNotFoundError(request.Id));

            var applyResult = bbq.Apply(new BbqStatusUpdated(request.GonnaHappen, request.TrincaWillPay));
            
            if (applyResult.IsFailed)
                return Result.Fail(applyResult.Errors);

            var lookups = await _snapshots.AsQueryable<Lookup>("Lookups").SingleOrDefaultAsync();

            foreach (var personId in lookups.PeopleIds)
            {
                var person = await _persons.GetAsync(personId);
                
                if(person is null)
                    return Result.Fail(new PersonNotFoundError(personId));

                switch (person.IsCoOwner, request.GonnaHappen)
                {
                    case (true, false):
                        var declineEvent = new InviteWasDeclined { InviteId = bbq.Id, PersonId = person.Id };
                        person.Apply(declineEvent);
                        break;
                    
                    case (false, true):
                        var inviteEvent = new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason);
                        person.Apply(inviteEvent);
                        break;
                }

                await _persons.SaveAsync(person);
            }

            await _repository.SaveAsync(bbq);
            return Result.Ok(bbq.TakeSnapshot());
        }
    }
}
