using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AmisPayroll.Entities.Entities;

namespace AmisPayroll.Application.Interfaces.Repositories
{
    public interface ISalaryCompositionRepository : IBaseRepository<SalaryComposition>
    {
        Task<bool> CheckDuplicateCodeAsync(string code);
        Task<bool> CheckDuplicateCodeAsync(string code, Guid excludeId);

        Task<(IEnumerable<dynamic> Data, int TotalCount)> GetPagingAsync(int skip, int take, string? searchValue);
    }
}
