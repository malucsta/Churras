using CrossCutting;
using Domain.Bbqs;
using Domain.Bbqs.Errors;
using Domain.Bbqs.Events;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.Lookups;
using Domain.People;
using Domain.People.Errors;
using Domain.People.Events;
using Domain.People.Repositories;
using Eveneum;
using FluentResults;

namespace Application.UseCases.Bbqs
{
    public class CreateBbq : ICreateBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _people;
        private readonly IBbqRepository _repository;
        
        public CreateBbq(SnapshotStore snapshots, Person user, IBbqRepository repository, IPersonRepository people)
        {
            _user = user;
            _snapshots = snapshots;
            _repository = repository;
            _people = people;
        }

        public async Task<Result<object>> CreateBbqAsync(NewBbqRequest request)
        {
            var result = await _repository.ThereIsBbqAt(request.Date);
            if (result)
                return Result.Fail(new ConflictingBbqs(request.Date));
            
            var churras = new Bbq();
            var applyResult = churras.Apply(new ThereIsSomeoneElseInTheMood(Guid.NewGuid(), request.Date, request.Reason, request.IsTrincasPaying));

            if (applyResult.IsFailed)
                return Result.Fail(applyResult.Errors);

            await _repository.SaveAsync(churras, _user.Id);

            var churrasSnapshot = churras.TakeSnapshot();

            var Lookups = await _snapshots.AsQueryable<Lookup>("Lookups").SingleOrDefaultAsync();

            foreach (var personId in Lookups.ModeratorIds)
            {
                var header = await _people.GetHeaderAsync(personId);

                if (header is null)
                    return Result.Fail(new PersonNotFoundError(personId)); 

                var @event = new[] { new EventData(personId, new PersonHasBeenInvitedToBbq(churras.Id, churras.Date, churras.Reason), new { CreatedBy = _user.Id }, header.StreamHeader.Version, DateTime.Now.ToString()) };

                await _people.SaveAsync(personId, @event,_user.Id, header.StreamHeader.Version);
            }

            return Result.Ok(churrasSnapshot);
        }
    }
}
