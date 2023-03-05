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
    public class DeclineInvite : IDeclineInvite
    {
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbq;

        public DeclineInvite(IPersonRepository repository, IBbqRepository bbq)
        {
            _repository = repository;
            _bbq = bbq;
        }

        public async Task<Result<object>> Execute(DeclineAnswer answer)
        {
            var person = await _repository.GetAsync(answer.UserId);

            if (person == null)
                return Result.Fail(new PersonNotFoundError(answer.UserId));

            var @event = new InviteWasDeclined { InviteId = answer.InviteId, PersonId = person.Id };
            var invite = person.Invites.FirstOrDefault(x => x.Id == answer.InviteId);
            
            var result = person.Apply(@event);
            if (result.IsFailed)
                return Result.Fail(result.Errors);

            if (invite is not null && invite.Status == InviteStatus.Accepted)
            {
                var bbq = await _bbq.GetAsync(answer.InviteId);
                if (bbq is null)
                    return Result.Fail(new BbqNotFoundError(answer.InviteId));

                var bbqResult = bbq.Apply(@event);
                if (bbqResult.IsFailed)
                    return Result.Fail(bbqResult.Errors);

                await _bbq.SaveAsync(bbq);
            }

            await _repository.SaveAsync(person);

            return Result.Ok(person.TakeSnapshot());
        }
    }
}
