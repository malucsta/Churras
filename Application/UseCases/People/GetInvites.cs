using Domain.Common;
using Domain.People.Repositories;
using Domain.People;
using Domain.People.UseCases;
using FluentResults;
using Microsoft.Azure.Cosmos;
using Domain.People.Errors;

namespace Application.UseCases.People
{
    public class GetInvites : IGetInvites
    {
        private readonly IPersonRepository _repository;

        public GetInvites(IPersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<object>> Execute(GetInvitesRequest request)
        {
            var person = await _repository.GetAsync(request.UserId);
            //var person = await _peopleStore.ReadStream(_user.Id);
            var personEntity = new Person();

            if (person is null)
                return Result.Fail(new PersonNotFoundError(request.UserId));

            return Result.Ok(person.TakeSnapshot());
        }
    }
}
