using Eveneum;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Common
{
    internal abstract class StreamRepository<T> : IStreamRepository<T> where T : AggregateRoot, new()
    {
        protected IEventStore _eventStore;
        protected StreamRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public virtual async Task<T?> GetAsync(string streamId)
        {
            try
            {
                
                //var stream = await _eventStore.ReadStream(streamId, new ReadStreamOptions { MaxItemCount = int.MaxValue, IgnoreSnapshots = true });
                var stream = await _eventStore.ReadStream(streamId);

                var entity = new T();

                if (stream.Stream == null)
                    return null;

                var @events = stream.Stream.Value.Events.Select(@event => (IEvent)@event.Body);
                entity.Rehydrate(@events);
                return entity;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            
        }

        public async Task<StreamHeaderResponse> GetHeaderAsync(string streamId)
            => await _eventStore.ReadHeader(streamId);

        public virtual async Task SaveAsync(T entity)
            => await _eventStore.WriteToStream(entity.Id, entity.Changes.Select(evento => new EventData(entity.Id, evento, null, entity.Version, DateTime.Now.ToString())).ToArray(), expectedVersion: entity.Version == 0 ? (ulong?)null : entity.Version);

        public virtual async Task SaveAsync(T entity, string userId)
            => await _eventStore.WriteToStream(entity.Id, entity.Changes.Select(evento => new EventData(entity.Id, evento, new { CreatedBy = userId }, entity.Version, DateTime.Now.ToString())).ToArray(), expectedVersion: entity.Version == 0 ? (ulong?)null : entity.Version);

        public virtual async Task SaveAsync(string id, EventData[] events, string userId, ulong version)
            => await _eventStore.WriteToStream(id, events, expectedVersion: version == 0 ? (ulong?)null : version);
    }
}
