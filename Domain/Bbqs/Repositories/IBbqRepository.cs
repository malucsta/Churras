using Domain.Common;
using System.Threading.Tasks;
using System;

namespace Domain.Bbqs.Repositories
{
    public interface IBbqRepository : IStreamRepository<Bbq> 
    {
        Task<bool> ThereIsBbqAt(DateTime date);
    }
}