using Domain.Bbqs;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.People.Repositories;
using Domain.People;
using Eveneum;
using FluentResults;
using Domain.Common.Errors;
using Domain.Bbqs.Errors;
using Domain.People.Errors;

namespace Application.UseCases.Bbqs
{
    public class GetShoppingList : IGetShoppingList
    {
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _repository;
        public GetShoppingList(IBbqRepository bbqs, IPersonRepository repository)
        {
            _bbqs = bbqs;
            _repository = repository;
        }
        public async Task<Result<object?>> Execute(GetShoppingListRequest request)
        {
            var person = await _repository.GetAsync(request.UserId);
           
            if (person is null)
                return Result.Fail(new PersonNotFoundError(request.UserId));

            if (!person.IsCoOwner)
                return Result.Fail(new UnauthorizedOperation("GetShoppingList", request.UserId)); 

            var bbq = await _bbqs.GetAsync(request.Id);
            
            if (bbq is null)
                return Result.Fail(new BbqNotFoundError(request.Id));

            return Result.Ok(bbq.GetShoppingListSnapshot());
        }
    }
}
