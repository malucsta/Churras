using Domain.Bbqs;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.People.Repositories;
using Domain.People;
using FluentResults;
using Microsoft.Azure.Cosmos;
using Domain.Bbqs.Errors;
using Domain.People.Errors;

namespace Application.UseCases.Bbqs
{
    public class ListBbqs : IListBbqs
    {
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _repository;
        
        public ListBbqs(IPersonRepository repository, IBbqRepository bbqs)
        {
            _bbqs = bbqs;
            _repository = repository;
        }

        public async Task<Result<List<object>>> Execute(GetProposedBbqsRequest request)
        {
            var snapshots = new List<object>();
            var moderator = await _repository.GetAsync(request.UserId);
            
            if (moderator is null)
                return Result.Fail(new PersonNotFoundError(request.UserId));
            
            foreach (var bbqId in moderator.Invites.Where(i => i.Date > DateTime.Now).Select(o => o.Id).ToList())
            {
                var bbq = await _bbqs.GetAsync(bbqId);

                if (bbq is null)
                    return Result.Fail(new BbqNotFoundError(bbqId));

                if (bbq.Status != BbqStatus.ItsNotGonnaHappen)
                    snapshots.Add(bbq.TakeSnapshot());
            }

            return Result.Ok(snapshots);
        }
    }
}
