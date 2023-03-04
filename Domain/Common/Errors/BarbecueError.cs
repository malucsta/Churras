using FluentResults;
using System;
using System.Collections.Generic;

namespace Domain.Common.Errors
{
    public abstract class BarbecueError : IError
    {
        protected Dictionary<string, object> _metadata = new Dictionary<string, object>();
        protected string _message = string.Empty;

        public BarbecueError() { }

        public BarbecueError(string message, Dictionary<string, object>? metadata = null)
        {
            _message = message;

            if (metadata != null)
                _metadata = metadata;
        }

        public abstract string Code { get; }

        public List<IError> Reasons => throw new NotImplementedException();

        public virtual string Message => _message;

        public Dictionary<string, object> Metadata => _metadata;
    }
}
