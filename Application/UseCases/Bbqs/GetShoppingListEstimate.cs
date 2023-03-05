using Domain.Bbqs.Errors;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.Common.Errors;
using Domain.People.Errors;
using Domain.People.Repositories;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Bbqs
{
    public class GetShoppingListEstimate : IGetShoppingListEstimate
    {
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _repository; 
        
        public GetShoppingListEstimate(IBbqRepository bbqs, IPersonRepository repository)
        {
            _bbqs = bbqs;
            _repository = repository;
        }

        public async Task<Result<object>> Execute(GetShoppingListEstimateRequest request)
        {
            var person = await _repository.GetAsync(request.UserId);

            if (person is null)
                return Result.Fail(new PersonNotFoundError(request.UserId));

            if (!person.IsCoOwner)
                return Result.Fail(new UnauthorizedOperation("GetShoppingList", request.UserId));

            var bbq = await _bbqs.GetAsync(request.BbqId);
            
            if (bbq is null)
                return Result.Fail(new BbqNotFoundError(request.BbqId));

            return Result.Ok(bbq.GetShoppingListEstimateSnapshot(request.MeatPricePerKg, request.VegetablesPricePerKg));
        }
    }
}
