using CrossCutting;
using Domain.Bbqs.Events;
using Domain.Bbqs.Repositories;
using Domain.Lookups;
using Domain.People;
using Domain.People.Events;
using Domain.People.Repositories;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunModerateBbq
    {
        private readonly SnapshotStore _snapshots;
        private readonly IPersonRepository _persons;
        private readonly IBbqRepository _repository;

        public RunModerateBbq(IBbqRepository repository, SnapshotStore snapshots, IPersonRepository persons)
        {
            _persons = persons;
            _snapshots = snapshots;
            _repository = repository;
        }

        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var moderationRequest = await req.Body<ModerateBbqRequest>();

            var bbq = await _repository.GetAsync(id);

            bbq.Apply(new BbqStatusUpdated(moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay));

            var lookups = await _snapshots.AsQueryable<Lookup>("Lookups").SingleOrDefaultAsync();

            foreach (var personId in lookups.PeopleIds)
            {
                // MODIFIED: para não mandar mais para os moderadores
                var person = await _persons.GetAsync(personId);

                switch (person.IsCoOwner, moderationRequest.GonnaHappen)
                {
                    case (true, false):
                        var edeclineEvent = new InviteWasDeclined { InviteId = bbq.Id, PersonId = person.Id };
                        person.When(edeclineEvent);
                        person.Apply(edeclineEvent);
                        break;
                    case (false, true):
                        var inviteEvent = new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason);
                        person.Apply(inviteEvent);
                        break;
                }

                await _persons.SaveAsync(person);
            }

            await _repository.SaveAsync(bbq);

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, bbq.TakeSnapshot());
        }
    }
}
