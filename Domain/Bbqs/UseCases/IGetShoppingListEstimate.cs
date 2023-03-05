using FluentResults;
using System.Threading.Tasks;

namespace Domain.Bbqs.UseCases 
{
    public class GetShoppingListEstimateRequest
    {
        public string BbqId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal MeatPricePerKg { get; set; }
        public decimal VegetablesPricePerKg { get; set; }
    }

    public interface IGetShoppingListEstimate
    {
        Task<Result<object>> Execute(GetShoppingListEstimateRequest request);
    }
}
