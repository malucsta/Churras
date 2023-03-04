using System.Net;
using Domain.Bbqs;
using Domain.Bbqs.Repositories;
using Domain.People;
using Domain.People.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly Person _user;
        private readonly IBbqRepository _bbqs;
        private readonly IPersonRepository _repository;
        public RunGetProposedBbqs(IPersonRepository repository, IBbqRepository bbqs, Person user)
        {
            _user = user;
            _bbqs = bbqs;
            _repository = repository;
        }

        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            var snapshots = new List<object>();
            var moderator = await _repository.GetAsync(_user.Id);
            foreach (var bbqId in moderator.Invites.Where(i => i.Date > DateTime.Now).Select(o => o.Id).ToList())
            {
                var bbq = await _bbqs.GetAsync(bbqId);
                
                // MODIFIED: churrascos que n�o v�o acontecer n�o s�o listados
                if(bbq.Status != BbqStatus.ItsNotGonnaHappen) 
                    snapshots.Add(bbq.TakeSnapshot());
            }

            return await req.CreateResponse(HttpStatusCode.Created, snapshots);
        }
    }
}
