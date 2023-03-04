using Domain.Common.Errors;

namespace Domain.People.Errors
{
    public class PersonNotFoundError : BarbecueError
    {
        public PersonNotFoundError(string personId)
        {
            _message = $"Person with id {personId} was not found";
        }

        public override string Code => BarbecueErrorCode.RESOURCE_not_found;
    }
}
