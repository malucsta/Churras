using FluentResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Bbqs.UseCases
{
    public class GetProposedBbqsRequest
    {
        public string UserId { get; set; } = string.Empty;
    }

    public interface IListBbqs
    {
        Task<Result<List<object>>> Execute(GetProposedBbqsRequest request);
    }
}
