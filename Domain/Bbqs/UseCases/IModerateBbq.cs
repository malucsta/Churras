using FluentResults;
using System.Threading.Tasks;

namespace Domain.Bbqs.UseCases
{
    public class ModerateBbqRequest
    {
        public string Id { get; set; } = string.Empty;
        public bool GonnaHappen { get; set; }
        public bool TrincaWillPay { get; set; }
    }

    public interface IModerateBbq
    {
        Task<Result<object>> Execute(ModerateBbqRequest request);
    }
}
