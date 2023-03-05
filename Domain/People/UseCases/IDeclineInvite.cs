using FluentResults;
using System.Threading.Tasks;

namespace Domain.People.UseCases
{
    public class DeclineAnswer
    {
        public string InviteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public interface IDeclineInvite
    {
        Task<Result<object>> Execute(DeclineAnswer answer);
    }
}
