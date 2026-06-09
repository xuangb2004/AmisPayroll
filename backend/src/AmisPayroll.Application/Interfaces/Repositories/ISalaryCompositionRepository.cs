using System;
using System.Threading.Tasks;
using AmisPayroll.Entities.Entities; 

namespace AmisPayroll.Application.Interfaces.Repositories
{
    public interface ISalaryCompositionRepository : IBaseRepository<SalaryComposition>
    {
        Task<bool> CheckDuplicateCodeAsync(string code, Guid? currentId = null);
    }
}