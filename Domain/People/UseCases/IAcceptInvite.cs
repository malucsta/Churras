using FluentResults;
using System.Threading.Tasks;

namespace Domain.People.UseCases
{
    public class InviteAnswer
    {
        public string InviteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsVeg { get; set; }
    }

    public interface IAcceptInvite
    {
        Task<Result<object>> Execute(InviteAnswer answer);
    }
}
