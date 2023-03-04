using System.Threading.Tasks;

namespace Domain.Bbqs.UseCases
{
    public class GetShoppingListRequest
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public interface IGetShoppingList
    {
        Task<object?> Execute(GetShoppingListRequest request);
    }
}
