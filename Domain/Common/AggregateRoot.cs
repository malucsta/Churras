using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.ExceptionServices;
using FluentResults;
using Domain.Common.Errors;

namespace Domain.Common
{
    public abstract class AggregateRoot
    {
        public string Id { get; set; } = string.Empty;
        public ulong Version { get; private set; }
        public List<IEvent> Changes { get; }

        public AggregateRoot()
        {
            Changes = new List<IEvent>();
        }

        public void Rehydrate(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                var result = Mutate(@event);
                if (result.IsFailed)
                    return;
                
                Version += 1;
            }
        }

        public Result Apply(IEvent @event)
        {
            Changes.Add(@event);
            return Mutate(@event);
        }

        private Result Mutate(IEvent @event)
        {
            try
            {
                Result result = ((dynamic)this).When((dynamic)@event);
                return result;
            }
            catch (RuntimeBinderException ex)
            {
                Console.WriteLine($"@@@@@@@ When({@event.GetType()} @event) method must be implemented for type {GetType()}. @@@@@@@");
                ExceptionDispatchInfo.Capture(new RuntimeBinderException(string.Format("Must implement event handler", @event.GetType(), GetType()), ex)).Throw();
                return Result.Fail(new EventHandlerNotImplementedError(GetType(), @event.GetType()));
            }
        }
    }
}
