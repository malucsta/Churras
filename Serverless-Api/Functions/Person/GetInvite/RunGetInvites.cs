using Domain.Common;
using Domain.People;
using Domain.People.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetInvites
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;
        private readonly IEventStore<Person> _peopleStore;

        public RunGetInvites(Person user, IPersonRepository repository,  IEventStore<Person> peopleStore)
        {
            _user = user;
            _repository = repository;
            _peopleStore = peopleStore;
        }

        [Function(nameof(RunGetInvites))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/invites")] HttpRequestData req)
        {
            //var person = await _repository.GetAsync(_user.Id);
            var person = await _peopleStore.ReadStream(_user.Id);
            var personEntity = new Person();

            if (person.Stream == null)
                return null;

            var @events = person.Stream.Value.Events.Select(@event => (IEvent)@event.Body);
            personEntity.Rehydrate(@events);

            if (person == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            //return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
            return await req.CreateResponse(System.Net.HttpStatusCode.OK, personEntity.TakeSnapshot());
        }
    }
}
