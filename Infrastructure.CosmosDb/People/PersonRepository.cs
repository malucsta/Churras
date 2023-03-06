using Domain.Common;
using Infrastructure.CosmosDb.EventsStore;

namespace Domain.People.Repositories
{
    internal class PersonRepository : StreamRepository<Person>, IPersonRepository
    {
        public PersonRepository(IEventStore<Person> eventStore) : base(eventStore) { }
    }
}
