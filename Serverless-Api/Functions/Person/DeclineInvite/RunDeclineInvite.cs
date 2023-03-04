using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.People;
using Domain.People.Repositories;
using Domain.People.Events;
using Domain.Bbqs.Repositories;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbq;

        public RunDeclineInvite(Person user, IPersonRepository repository, IBbqRepository bbq)
        {
            _user = user;
            _repository = repository;
            _bbq = bbq;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var person = await _repository.GetAsync(_user.Id);

            if (person == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            var @event = new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id };
            var invite = person.Invites.FirstOrDefault(x => x.Id == inviteId);
            
            if (invite is not null && invite.Status == InviteStatus.Accepted)
            {
                var bbq = await _bbq.GetAsync(inviteId);
                if (bbq is null)
                    return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);

                bbq.Apply(@event);
                await _bbq.SaveAsync(bbq);
            }

            person.Apply(@event);
            await _repository.SaveAsync(person);

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
        }
    }
}
