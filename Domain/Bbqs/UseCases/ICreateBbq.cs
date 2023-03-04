using FluentResults;
using System;
using System.Threading.Tasks;

namespace Domain.Bbqs.UseCases 
{
    public class NewBbqRequest
    {
        public NewBbqRequest(DateTime date, string reason, bool isTrincaPaying)
        {
            Date = date;
            Reason = reason;
            IsTrincasPaying = isTrincaPaying;
        }

        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public bool IsTrincasPaying { get; set; }
    }

    public interface ICreateBbq
    {
        Task<Result<object>> CreateBbqAsync(NewBbqRequest request);
    }
}
