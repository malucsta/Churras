using CrossCutting;
using Domain.Bbqs;
using Domain.Bbqs.Events;
using Domain.Bbqs.Repositories;
using Domain.Common;
using Eveneum;
using Infrastructure.CosmosDb.EventsStore;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.CosmosDb.Bbqs
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

            var queryable = Container.GetItemLinqQueryable<EventDocumentDto>(true)
                .Where(e => e.BodyType.Contains(eventType) && e.Body != null && e.Body.Date == date)
                .Select(e => e.Body);

            var results = await queryable.ToListAsync();

            if (results.Any())
                return true;

            return false;
        }
    }

    internal class EventDocumentDto
    {
        public string Id { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string StreamId { get; set; } = string.Empty;
        public int Version { get; set; }
        public string MetadataType { get; set; } = string.Empty;
        public object? Metadata { get; set; }
        public string BodyType { get; set; } = string.Empty;
        public ThereIsSomeoneElseInTheMood? Body { get; set; }
        public double SortOrder { get; set; }
        public bool Deleted { get; set; }
    }
}
