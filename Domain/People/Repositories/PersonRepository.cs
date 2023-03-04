using Domain.Common;
using Microsoft.Azure.Cosmos;
using System.Security.Cryptography.X509Certificates;

namespace Domain.People.Repositories
{
    internal class PersonRepository : StreamRepository<Person>, IPersonRepository
    {
        public PersonRepository(IEventStore<Person> eventStore) : base(eventStore) { }
    }
}
