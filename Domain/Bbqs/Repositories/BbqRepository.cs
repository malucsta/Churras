using CrossCutting;
using Domain.Bbqs.Events;
using Domain.Common;
using Eveneum;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Bbqs.Repositories
{
    internal class BbqRepository : StreamRepository<Bbq>, IBbqRepository
    {
        public BbqRepository(IEventStore<Bbq> eventStore) : base(eventStore) 
        {
            _eventStore = eventStore;
        }

        public async Task<bool> ThereIsBbqAt(DateTime date)
        {
            CosmosClient Client = new CosmosClient(Environment.GetEnvironmentVariable(nameof(EventStore)));
            Container Container = Client.GetContainer("Churras", "Bbqs");

            string eventType = "Domain.Bbqs.Events.ThereIsSomeoneElseInTheMood";

            var queryable = Container.GetItemLinqQueryable<EventDto>(true)
                .Where(e => e.BodyType.Contains(eventType) && e.Body.Date == date)
                .Select(e => e.Body);

            var results = await queryable.ToListAsync();

            if (results.Any())
                return true;

            return false;
        }
    }

    internal class EventDto
    {
        public string Id { get; set; }
        public string DocumentType { get; set; }
        public string StreamId { get; set; }
        public int Version { get; set; }
        public string MetadataType { get; set; }
        public object Metadata { get; set; }
        public string BodyType { get; set; }
        public ThereIsSomeoneElseInTheMood Body { get; set; }
        public double SortOrder { get; set; }
        public bool Deleted { get; set; }
    }
}
