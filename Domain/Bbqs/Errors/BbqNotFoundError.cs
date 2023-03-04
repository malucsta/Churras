using Domain.Common.Errors;

namespace Domain.Bbqs.Errors
{
    public class BbqNotFoundError : BarbecueError
    {
        public BbqNotFoundError(string id)
        {
            _message = $"Barbecue with id {id} was not found";
        }

        public override string Code => BarbecueErrorCode.RESOURCE_not_found;
    }
}
