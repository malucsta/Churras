using FluentResults;
using System.Threading.Tasks;

namespace Domain.People.UseCases
{
    public class GetInvitesRequest
    {
        public string UserId { get; set; } = string.Empty;
    }

    public interface IGetInvites
    {
        Task<Result<object>> Execute(GetInvitesRequest request);
    }
}
