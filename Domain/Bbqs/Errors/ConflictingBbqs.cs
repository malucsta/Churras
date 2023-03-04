using Domain.Common.Errors;
using System;

namespace Domain.Bbqs.Errors
{
    public class ConflictingBbqs : BarbecueError
    {
        public ConflictingBbqs(DateTime date)
        {
            _message = $"Barbecue at date {date} conflicts with another one";
        }

        public override string Code => BarbecueErrorCode.RESOURCE_conflict;
    }
}
