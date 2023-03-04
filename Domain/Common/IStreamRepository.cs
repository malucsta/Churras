using Eveneum;
using System.Threading.Tasks;

namespace Domain.Common
{
    public interface IStreamRepository<T> where T : AggregateRoot, new()
    {
        Task<StreamHeaderResponse> GetHeaderAsync(string streamId);
        Task<T?> GetAsync(string streamId);
        Task SaveAsync(T entity);
        Task SaveAsync(T entity, string userId);
        Task SaveAsync(string id, EventData[] events, string userId, ulong version);
    }
}