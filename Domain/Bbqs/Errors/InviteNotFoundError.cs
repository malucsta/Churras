using Domain.Common.Errors;

namespace Domain.Bbqs.Errors
{
    public class InviteNotFoundError : BarbecueError
    {
        public InviteNotFoundError(string personId)
        {
            _message = $"Invite for person with id {personId} was not found";
        }

        public override string Code => BarbecueErrorCode.RESOURCE_not_found;
    }
}
