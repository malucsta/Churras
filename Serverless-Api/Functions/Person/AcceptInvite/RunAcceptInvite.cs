using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.People;
using Domain.People.Repositories;
using Domain.People.Events;
using Domain.Bbqs.Repositories;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqs;
        public RunAcceptInvite(IBbqRepository bbqs, IPersonRepository repository, Person user)
        {
            _user = user;
           _repository = repository;
            _bbqs = bbqs;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>();

            var person = await _repository.GetAsync(_user.Id);
            var bbq = await _bbqs.GetAsync(inviteId);

            var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = answer.IsVeg, PersonId = person.Id };
            person.Apply(@event);
            bbq.When(@event);
            bbq.Apply(@event);

            await _repository.SaveAsync(person);
            await _bbqs.SaveAsync(bbq);

            //TODO: implementar efeito do aceite do convite no churrasco
            //quando tiver 7 pessoas ele está confirmado

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
        }
    }
}
