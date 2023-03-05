namespace Domain.Common.Errors
{
    public class UnauthorizedOperation : BarbecueError
    {
        public UnauthorizedOperation(string operation, string userId)
        {
            _message = $"Operation {operation} is not authorized for user with id {userId}";
        }

        public override string Code => BarbecueErrorCode.OPERATION_unauthorized;
    }
}
