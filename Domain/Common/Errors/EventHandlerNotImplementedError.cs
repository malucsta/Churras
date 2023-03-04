using Microsoft.Extensions.Logging;
using System;

namespace Domain.Common.Errors
{
    public class EventHandlerNotImplementedError : BarbecueError
    {
        public EventHandlerNotImplementedError(Type entityType, Type eventType)
        {
            _message = $"@@@@@@@ When({eventType} @event) method must be implemented for type {entityType}. @@@@@@@";
        }

        public override string Code => BarbecueErrorCode.RESOURCE_not_found;
    }
}
