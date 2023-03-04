using Domain.Common.Errors;
using System;

namespace Domain.Bbqs.Errors
{
    public class ConflictingBbqsError : BarbecueError
    {
        public ConflictingBbqsError(DateTime date)
        {
            _message = $"Barbecue at date {date} conflicts with another one";
        }

        public override string Code => BarbecueErrorCode.RESOURCE_conflict;
    }
}
