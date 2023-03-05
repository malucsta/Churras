using Domain.Bbqs.Repositories;
using Domain.People.Repositories;
using Domain.People;
using Domain.People.UseCases;
using FluentResults;
using Domain.People.Events;
using Microsoft.Azure.Cosmos;
using Domain.People.Errors;
using Domain.Bbqs.Errors;

namespace Application.UseCases.People
{
    public class AcceptInvite : IAcceptInvite
    {
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqs;
        public AcceptInvite(IBbqRepository bbqs, IPersonRepository repository)
        {
            _repository = repository;
            _bbqs = bbqs;
        }

        public async Task<Result<object>> Execute(InviteAnswer answer)
        {
            var person = await _repository.GetAsync(answer.UserId);
            if (person is null)
                return Result.Fail(new PersonNotFoundError(answer.UserId));

            var bbq = await _bbqs.GetAsync(answer.InviteId);
            if (bbq is null)
                return Result.Fail(new BbqNotFoundError(answer.InviteId));

            var @event = new InviteWasAccepted { InviteId = answer.InviteId, IsVeg = answer.IsVeg, PersonId = person.Id };
            person.Apply(@event);
            bbq.Apply(@event);

            await _repository.SaveAsync(person);
            await _bbqs.SaveAsync(bbq);

            return Result.Ok(person.TakeSnapshot());
        }
    }
}
