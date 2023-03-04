using Domain.Bbqs;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.People.Repositories;
using Domain.People;
using Eveneum;

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
        public async Task<object?> Execute(GetShoppingListRequest request)
        {
            var person = await _repository.GetAsync(request.UserId);

            if (!person.IsCoOwner)
                return null; 

            var bbq = await _bbqs.GetAsync(request.Id);
            return bbq.GetShoppingList();
        }
    }
}
